module Multidata.Stv.StvCalculator

open StvModels

let calculateDroopQuota numberOfSeats totalValidPoll = 
    (float totalValidPoll + float 1) / (float numberOfSeats + float 1) |> floor |> int

// let calculateNumberOfVotes (voteList: Ballot) =
//     let filterFirstVotes =  List.filter (fun x -> x.preference = 1)
//     let firstPlaces = List.collect (fun x -> filterFirstVotes x) voteList

let aggregateVotes (voteList: Ballot list) =
    List.countBy (id) voteList 
     |> List.map (fun (b, c) -> 
         {ballot = b; numberOfVotes = c})


let rec iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResult : PollResult =



    if List.length pollResult.items = numberOfSeats then
        pollResult
    else
        iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResult 

// Only valid and sorted ballots
let mainCaluclation (poll: Poll) (voteList: Ballot list) : PollResult =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll
    let aggregatedVotes = aggregateVotes voteList
    let pollResult = {items = []}
    iterationLoop poll.numberOfSeats droopQuota aggregatedVotes pollResult
