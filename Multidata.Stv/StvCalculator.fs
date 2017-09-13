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
    let firstPlaces = List.map (fun x -> (getBallotFirstCandidateId 1 x.ballot, x.numberOfVotes)) aggregatedVoteList
    List.groupBy (fun (a, b) -> a) firstPlaces
    |> List.map (fun (key , values) -> (key, values |> List.sumBy snd))

let rec iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResult : PollResult =
    let numberResults = List.length pollResult.items
    match numberResults with
    //| 0 -> calculateFirstPoll 
    | numberOfSeats -> pollResult
    | _ -> iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResult

// Only valid and sorted ballots
let mainCaluclation (poll: Poll) (voteList: Ballot list) : PollResult =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll
    let aggregatedVotes = aggregateVotes voteList
    let pollResult = {items = []}
    iterationLoop poll.numberOfSeats droopQuota aggregatedVotes pollResult
