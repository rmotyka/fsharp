module Multidata.Stv.Test.StvCalculator

open System
open Xunit
open Multidata.Stv.StvModels
open Multidata.Stv.StvCalculator

[<Fact>]
let ``My test`` () =
    Assert.True(true)

[<Fact>]
let ``droop quota`` () =
    let res = calculateDroopQuota 2 100
    Assert.Equal(33, res)

[<Fact>]
let ``aggregate votes`` () =
    let ballotList = [
        [
            {VoteItem.candidateId = 1; preference = 1};
            {VoteItem.candidateId = 2; preference = 2};
        ];
        [
            {VoteItem.candidateId = 1; preference = 1};
            {VoteItem.candidateId = 2; preference = 2};
        ]
    ]
    let res = aggregateVotes ballotList
    printfn "Aggregate: %A" res
    Assert.True(true)

[<Fact>]
let ``mainCaluclation`` () =
    let candidate1 = {Candidate.candidateId = 1; Candidate.name = "Smith"}
    let candidate2 = {Candidate.candidateId = 2; Candidate.name = "Gordon"}
    let candidates = [candidate1; candidate2]
    let poll = {numberOfSeats = 12; candidates = candidates}
    let voteList = [[{candidateId = 1; preference = 1}]]
    let res = mainCaluclation poll voteList

    let items = res.items

    Assert.Equal(1, items.Item(0).candidateId)