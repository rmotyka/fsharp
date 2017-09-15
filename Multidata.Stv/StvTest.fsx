#r "bin\\Debug\\netcoreapp2.0\\Multidata.Stv.dll"

open Multidata.Stv.StvModels
open Multidata.Stv.StvCalculator
let candidates = [
    {Candidate.candidateId = 1; Candidate.name = "Andrea"};
    {Candidate.candidateId = 2; Candidate.name = "Brad"};
    {Candidate.candidateId = 3; Candidate.name = "Carter"};
    {Candidate.candidateId = 4; Candidate.name = "Delilah"};
 ]
let poll = {numberOfSeats = 2; candidates = candidates}
let voteList = [
    for i in 1 .. 16 -> [1; 2; 3; 4];
    for i in 1 .. 24 -> [1; 3; 2; 4];
    for i in 1 .. 17 -> [4; 1; 2; 3];
]
let res = mainCalculation poll voteList
printfn "Aggregate: %A" res