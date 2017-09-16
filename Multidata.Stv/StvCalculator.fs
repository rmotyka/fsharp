module Multidata.Stv.StvCalculator

open StvModels
open Multidata.Stv.StvQuota

let aggregateVotes (voteList: Ballot list) =
    List.countBy (id) voteList 
     |> List.map (fun (b, c) -> {ballot = b; numberOfVotes = c})

let getBallotFirstCandidateId (preference: int) (ballot: Ballot) = 
   List.item (preference - 1) ballot

let sumVotes (preference: int) (aggregatedVoteList: AggregatedVote list) =
    List.map (fun x -> (getBallotFirstCandidateId 1 x.ballot, x.numberOfVotes)) aggregatedVoteList // simplify the structure
    |> List.groupBy (fun (a, b) -> a) // aggregate by candidateId
    |> List.map (fun (key , values) -> (key, List.sumBy snd values)) // sum votes for candidate - I don't like: List.sumBy snd values

let verifyPollResult droopQuota (pollResultItemList: PollResultItem list) : PollResultItem list = 
    pollResultItemList |> List.map (fun x -> 
        if x.numberOfVotes >= droopQuota then  {x with elected = true }
        else {x with elected = false }) 

let voteSumToPollResult votesSum = 
    List.map (fun (c, v) -> {candidateId = c; numberOfVotes = v; elected = true}) votesSum

let isPollFinished numberOfSeats pollResultItemList =
    List.filter (fun x -> x.elected) pollResultItemList
    |> List.length >= numberOfSeats

let getSurplus droopQuota (pollResultItemList: PollResultItem list) = 
    pollResultItemList
    |> List.filter (fun x -> x.numberOfVotes > droopQuota)
    |> List.map (fun x -> (x.candidateId, x.numberOfVotes - droopQuota))

let getAggregatedVoteWhereCandidateIsOnPosiotion position aggregatedVoteList candidateId = 
    List.filter (fun x -> List.item (position - 1) x.ballot = candidateId) aggregatedVoteList

let getNextCandidateId ballot candidateId = 
    let maybeIndex = List.tryFindIndex (fun x -> x = candidateId) ballot
    match maybeIndex with
    | Some i -> List.tryItem (i + 1) ballot
    | None -> None

let getCandidateTotalVotes pollResultItemList candidateId = 
    let winnersResult = List.find (fun x -> x.candidateId = candidateId) pollResultItemList
    winnersResult.numberOfVotes

let calculateSurplusVotesToAdd votesForTheNextPreference totalWinnerVotes surplusVotes = 
    (int)(((float)votesForTheNextPreference / (float)totalWinnerVotes) * (float)surplusVotes)

let addNumberOfVotesToResult pollResultItemList candidateId votesToAdd = 
    List.map (fun x -> 
    if x.candidateId = candidateId then
        {candidateId = x.candidateId; numberOfVotes = x.numberOfVotes + votesToAdd; elected = x.elected}
    else
        x
    ) pollResultItemList

let addOneSurplus aggregatedVoteList pollResultItemList surplus = 
    let (winnerCandidateId, surplusNumberOfVotes) = surplus
    let winnerTotalVotes = getCandidateTotalVotes pollResultItemList winnerCandidateId
    let position = 1 // TODO: get from arguments
    let aggregateVotes = getAggregatedVoteWhereCandidateIsOnPosiotion position aggregatedVoteList winnerCandidateId
    List.fold (fun acc aggregateVote -> 
        let maybeNextCandidateId = getNextCandidateId aggregateVote.ballot winnerCandidateId
        match maybeNextCandidateId with
        | Some nextCandidateId -> 
            let votesToAdd = calculateSurplusVotesToAdd aggregateVote.numberOfVotes winnerTotalVotes surplusNumberOfVotes
            let tempPollResult = addNumberOfVotesToResult acc nextCandidateId votesToAdd
            addNumberOfVotesToResult tempPollResult winnerCandidateId -votesToAdd
        | None -> acc
    ) pollResultItemList aggregateVotes

// TODO: test
let addSurplus aggregatedVoteList pollResultItemList surplusList =
    List.fold (fun acc surplus -> addOneSurplus aggregatedVoteList acc surplus) pollResultItemList surplusList

let rec iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResultItemList =
    let pollResultItemList = verifyPollResult droopQuota pollResultItemList
    if isPollFinished numberOfSeats pollResultItemList then
        pollResultItemList
    else
        let surplusList = getSurplus droopQuota pollResultItemList
        if List.isEmpty surplusList then
            // TODO: eliminate last candidate here
            iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResultItemList
        else
            let pollResultItemList = addSurplus aggregatedVoteList pollResultItemList surplusList
            iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResultItemList

// Only valid and sorted ballots
let mainCalculation (poll: Poll) (voteList: Ballot list) =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll
    let aggregatedVotes = aggregateVotes voteList
    // first round
    let votesSum = sumVotes 1 aggregatedVotes
    let pollResultItemList = voteSumToPollResult votesSum
    iterationLoop poll.numberOfSeats droopQuota aggregatedVotes pollResultItemList
