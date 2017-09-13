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
let ``getBallotFirstCandidateId`` () =
    let ballot = [
        {VoteItem.candidateId = 1; preference = 2};
        {VoteItem.candidateId = 2; preference = 1}
    ]

    let res = getBallotFirstCandidateId 1 ballot
    Assert.Equal(2, res)

[<Fact>]
let ``sumVotes`` () =
    let aggregatedVoteList = [
        {AggregatedVote.ballot = [
                {VoteItem.candidateId = 1; preference = 1};
                {VoteItem.candidateId = 2; preference = 2}
            ]; numberOfVotes = 1
        };
        {AggregatedVote.ballot = [
            {VoteItem.candidateId = 1; preference = 1};
            {VoteItem.candidateId = 2; preference = 2}
        ]; numberOfVotes = 2
        };
        {AggregatedVote.ballot = [
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

[<Fact>]
let ``isPollFinished true`` () = 
    let pollResult = {items = [
        {candidateId = 1; numberOfVotes = 2; elected = true};
        {candidateId = 2; numberOfVotes = 2; elected = true};
        {candidateId = 3; numberOfVotes = 2; elected = true};
     ] }
    let res = isPollFinished 2 pollResult
    Assert.True(res)

let ``isPollFinished false`` () = 
    let pollResult = {items = [
        {candidateId = 1; numberOfVotes = 2; elected = true};
     ] }
    let res = isPollFinished 2 pollResult
    Assert.False(res)

[<Fact>]
let ``getSurplus`` () =
    let pollResult = {items = [
        {candidateId = 1; numberOfVotes = 4; elected = true};
        {candidateId = 2; numberOfVotes = 5; elected = true};
        {candidateId = 3; numberOfVotes = 6; elected = true};
     ] }
    let res = getSurplus 5 pollResult
    Assert.Equal(1, List.length res)
    let (c, v) = res.[0]
    Assert.Equal(3, c)
    Assert.Equal(1, v)

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





(* // TODO: fix that test
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