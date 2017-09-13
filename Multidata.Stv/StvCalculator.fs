module Multidata.Stv.StvCalculator

open StvModels

let calculateDroopQuota numberOfSeats totalValidPoll = 
    (float totalValidPoll + float 1) / (float numberOfSeats + float 1) |> ceil |> int

// let calculateNumberOfVotes (voteList: Ballot) =
//     let filterFirstVotes =  List.filter (fun x -> x.preference = 1)
//     let firstPlaces = List.collect (fun x -> filterFirstVotes x) voteList

let aggregateVotes (voteList: Ballot list) =
    List.countBy (id) voteList 
     |> List.map (fun (b, c) -> 
         {ballot = b; numberOfVotes = c})


let sumVotes preference (aggregatedVoteList: AggregatedVote list) =
    let aggregteFun acc x =
        let item = List.find (fun x )

    List.fold (aggregteFun) [] aggregatedVoteList

    // let getBallotFirstCandidate ballot = 
    //     let voteItem = List.find (fun x -> x.preference = preference) ballot
    //     voteItem.candidateId

    // let firstPlaces = List.map (fun x -> (getBallotFirstCandidate x.ballot, x.numberOfVotes)) aggregatedVoteList
    // List.sumBy  firstPlaces // TODO: aggregate

let rec iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResult : PollResult =
    let numberResults = List.length pollResult.items
    match numberResults then
    | 0 -> calculateFirstPoll 
    | numberOfSeats -> pollResult
    | _ -> iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResult
    


// Only valid and sorted ballots
let mainCaluclation (poll: Poll) (voteList: Ballot list) : PollResult =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll
    let aggregatedVotes = aggregateVotes voteList
    let pollResult = {items = []}
    iterationLoop poll.numberOfSeats droopQuota aggregatedVotes pollResult
