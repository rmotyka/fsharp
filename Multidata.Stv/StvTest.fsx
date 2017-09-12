#r "bin\\Debug\\netcoreapp2.0\\Multidata.Stv.dll"

open Multidata.Stv.StvModels
open Multidata.Stv.StvCalculator
let ballotList = [
    [
        {VoteItem.candidateId = 1; preference = 1};
        {VoteItem.candidateId = 2; preference = 2};
    ];
    [
        {VoteItem.candidateId = 1; preference = 1};
        {VoteItem.candidateId = 2; preference = 2};
    ];
    [
        {VoteItem.candidateId = 1; preference = 1};
        {VoteItem.candidateId = 3; preference = 2};
    ]    
]

type AggregatedVote = {ballot: Ballot; numberOfVotes: int}

let aggregateVotes (voteList: Ballot list) =
    List.countBy (id) voteList 
     |> List.map (fun (b, c) -> 
         {ballot = b; numberOfVotes = c})

//let res = List.countBy (id) ballotList

let res = aggregateVotes ballotList
printfn "Aggregate: %A" res