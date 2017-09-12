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


//let iterationLoop droopQuota aggregatedVoteList =
    // result

// Only valid votesBallot
let mainCaluclation (poll: Poll) (voteList: Ballot list) : PollResult =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll




    // temporary result
    let pollResult = {items = [{candidateId = 1; numberOfVotes = 100; elected = true}]}
    pollResult
