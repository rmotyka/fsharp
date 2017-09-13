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
    Assert.Equal(34, res)

[<Fact>]
let ``sumVotes`` () =
    let aggregatedVoteList = [
        {
            ballot = [
                {VoteItem.candidateId = 1; preference = 1};
                {VoteItem.candidateId = 2; preference = 2}
            ]; numberOfVotes = 1
        };
        {ballot = [
            {VoteItem.candidateId = 1; preference = 1};
            {VoteItem.candidateId = 2; preference = 2}
        ]; numberOfVotes = 2
        };
        {ballot = [
            {VoteItem.candidateId = 3; preference = 1};
            {VoteItem.candidateId = 4; preference = 2}
        ]; numberOfVotes = 3
        }              
    ]

    let res = Multidata.Stv.StvCalculator.sumVotes 1 aggregatedVoteList
    let (c1, v1) = res.[0]
    Assert.Equal(1, c1)
    Assert.Equal(3, v1)
    let (c3, v3) = res.[1]
    Assert.Equal(3, c3)
    Assert.Equal(3, v3)    
(*
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
        ];
        [
            {VoteItem.candidateId = 1; preference = 1};
            {VoteItem.candidateId = 3; preference = 2};
        ]    
    ]

    let res = aggregateVotes ballotList

    let numberOfAggregates = List.length res
    Assert.Equal(2, numberOfAggregates)
    Assert.Equal(2, res.[0].numberOfVotes)
    Assert.Equal(1, res.[1].numberOfVotes)

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

    *)