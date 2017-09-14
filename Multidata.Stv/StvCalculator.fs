module Multidata.Stv.StvCalculator

open StvModels

let calculateDroopQuota numberOfSeats totalValidPoll = 
    (float totalValidPoll + float 1) / (float numberOfSeats + float 1) |> ceil |> int

// let calculateNumberOfVotes (voteList: Ballot) =
//     let filterFirstVotes =  List.filter (fun x -> x.preference = 1)
//     let firstPlaces = List.collect (fun x -> filterFirstVotes x) voteList

let aggregateVotes (voteList: Ballot list) =
    List.countBy (id) voteList 
     |> List.map (fun (b, c) -> {ballot = b; numberOfVotes = c})

let getBallotFirstCandidateId (preference: int) (ballot: Ballot) = 
   List.item (preference - 1) ballot

let sumVotes (preference: int) (aggregatedVoteList: AggregatedVote list) =
    List.map (fun x -> (getBallotFirstCandidateId 1 x.ballot, x.numberOfVotes)) aggregatedVoteList // simplify the structure
    |> List.groupBy (fun (a, b) -> a) // aggregate by candidateId
    |> List.map (fun (key , values) -> (key, List.sumBy snd values)) // sum votes for candidate - I don't like: List.sumBy snd values

let voteSumToPollResult droopQuota votesSum = 
    let filteredItems = List.filter (fun (c, v) -> v >= droopQuota) votesSum
    List.map (fun (c, v) -> {candidateId = c; numberOfVotes = v; elected = true}) filteredItems

let isPollFinished numberOfSeats pollResultItemList =
    List.filter (fun x -> x.elected) pollResultItemList
    |> List.length >= numberOfSeats

let getSurplus droopQuota (pollResultItemList: PollResultItem list) = 
    pollResultItemList
    |> List.filter (fun x -> x.numberOfVotes > droopQuota)
    |> List.map (fun x -> (x.candidateId, x.numberOfVotes - droopQuota))

// TODO: test
let getAggregatedVoteWhereCandidateIsOnPosiotion position aggregatedVoteList candidateId = 
    List.filter (fun x -> List.item (position - 1) x.ballot = candidateId) aggregatedVoteList

// TODO: test
let getNextCandidateId ballot candidateId = 
    let maybeIndex = List.tryFindIndex (fun x -> x = candidateId) ballot
    match maybeIndex with
    | Some i -> List.tryItem (i + 1) ballot
    | None -> None

// TODO: test
let getCandidateTotalVotes pollResultItemList candidateId = 
    let winnersResult = List.find (fun x -> x.candidateId = candidateId) pollResultItemList
    winnersResult.numberOfVotes

// TODO: test
let calculateSurplusVotesToAdd votesForTheNextPreference totalWinnerVotes surplusVotes = 
    (votesForTheNextPreference / totalWinnerVotes) * surplusVotes

// TODO: test
let addNumberOfVotesToResult pollResultItemList candidateId votesToAdd = 
    List.map (fun x -> 
    if x.candidateId = candidateId then
        {candidateId = x.candidateId; numberOfVotes = x.numberOfVotes + votesToAdd; elected = false}
    else
        x
    ) pollResultItemList

// TODO: test
let addOneSurplus aggregatedVoteList pollResultItemList surplus = 
    let (winnerCandidateId, surplusNumberOfVotes) = surplus
    let winnerTotalVotes = getCandidateTotalVotes pollResultItemList winnerCandidateId
    let position = 1 // TODO: get from arguments
    let aggregateVotes = getAggregatedVoteWhereCandidateIsOnPosiotion position aggregatedVoteList winnerCandidateId
    List.fold (fun acc aggregateVote -> 
        let maybeNextCandidateId = getNextCandidateId aggregateVote.ballot winnerCandidateId
        match maybeNextCandidateId with
        | Some nextCandidateId -> 
            calculateSurplusVotesToAdd aggregateVote.numberOfVotes winnerTotalVotes surplusNumberOfVotes
            |> addNumberOfVotesToResult acc nextCandidateId
        | None -> acc
    ) pollResultItemList aggregateVotes

// TODO: test
let addSurplus aggregatedVoteList pollResultItemList surplusList =
    List.fold (fun acc surplus -> addOneSurplus aggregatedVoteList acc surplus) pollResultItemList surplusList

let rec iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResultItemList =
    let surplusList = getSurplus droopQuota pollResultItemList
    let pollResultWithSurplus = addSurplus aggregatedVoteList pollResultItemList surplusList

    let numberResults = List.length pollResultItemList
    match numberResults with
    //| 0 -> calculateFirstPoll 
    | numberOfSeats -> pollResultItemList
    | _ -> iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResultItemList

// Only valid and sorted ballots
let mainCalculation (poll: Poll) (voteList: Ballot list) =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll
    let aggregatedVotes = aggregateVotes voteList
    // first round
    let votesSum = sumVotes 1 aggregatedVotes
    let pollResult = voteSumToPollResult droopQuota votesSum
    let pollFinished = isPollFinished poll.numberOfSeats pollResult
    if pollFinished then
        pollResult
    else
        iterationLoop poll.numberOfSeats droopQuota aggregatedVotes pollResult
