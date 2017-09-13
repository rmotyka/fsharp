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

let getBallotFirstCandidateId preference ballot = 
   let voteItem = List.find (fun x -> x.preference = preference) ballot
   voteItem.candidateId

let sumVotes (preference: int) (aggregatedVoteList: AggregatedVote list) =
    List.map (fun x -> (getBallotFirstCandidateId 1 x.ballot, x.numberOfVotes)) aggregatedVoteList // simplify the structure
    |> List.groupBy (fun (a, b) -> a) // aggregate by candidateId
    |> List.map (fun (key , values) -> (key, List.sumBy snd values)) // sum votes for candidate - I don't like: List.sumBy snd values

let rec iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResult : PollResult =
    
    
    let numberResults = List.length pollResult.items
    match numberResults with
    //| 0 -> calculateFirstPoll 
    | numberOfSeats -> pollResult
    | _ -> iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResult

let voteSumToPollResult droopQuota votesSum : PollResult = 
    let filteredItems = List.filter (fun (c, v) -> v >= droopQuota) votesSum
    let pollResultList = List.map (fun (c, v) -> {candidateId = c; numberOfVotes = v; elected = true}) filteredItems

    let pollResult = {items = pollResultList}
    pollResult

// Only valid and sorted ballots
let mainCaluclation (poll: Poll) (voteList: Ballot list) : PollResult =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll
    let aggregatedVotes = aggregateVotes voteList
    // first round
    let votesSum = sumVotes 1 aggregatedVotes
    let pollResult = voteSumToPollResult droopQuota votesSum


    iterationLoop poll.numberOfSeats droopQuota aggregatedVotes pollResult
