module Multidata.Stv.Test.StvCalculatorTest

open System
open Xunit
open Multidata.Stv.StvModels
open Multidata.Stv.StvCalculator

[<Fact>]
let ``My test`` () =
    Assert.True(true)

[<Fact>]
let ``getBallotFirstCandidateId`` () =
    let ballot = [ 2; 1 ]

    let res = getBallotFirstCandidateId 1 ballot
    Assert.Equal(2, res)

[<Fact>]
let ``sumVotes`` () =
    let aggregatedVoteList = [
        {AggregatedVote.ballot = [ 1; 2 ]; numberOfVotes = 1};
        {AggregatedVote.ballot = [ 1; 2 ]; numberOfVotes = 2};
        {AggregatedVote.ballot = [ 3; 4 ]; numberOfVotes = 3}
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
    let pollResult = [
        {candidateId = 1; numberOfVotes = 2; elected = true};
        {candidateId = 2; numberOfVotes = 2; elected = true};
        {candidateId = 3; numberOfVotes = 2; elected = true};
     ]
    let res = isPollFinished 2 pollResult
    Assert.True(res)

let ``isPollFinished false`` () = 
    let pollResult = [
        {candidateId = 1; numberOfVotes = 2; elected = true};
     ]
    let res = isPollFinished 2 pollResult
    Assert.False(res)

[<Fact>]
let ``getSurplus`` () =
    let pollResult = [
        {candidateId = 1; numberOfVotes = 4; elected = true};
        {candidateId = 2; numberOfVotes = 5; elected = true};
        {candidateId = 3; numberOfVotes = 6; elected = true};
     ]
    let res = getSurplus 5 pollResult
    Assert.Equal(1, List.length res)
    let (c, v) = res.[0]
    Assert.Equal(3, c)
    Assert.Equal(1, v)

[<Fact>]
let ``aggregate votes`` () =
    let ballotList = [[1; 2]; [1; 2]; [1; 3]]

    let res = aggregateVotes ballotList

    let numberOfAggregates = List.length res
    Assert.Equal(2, numberOfAggregates)
    Assert.Equal(2, res.[0].numberOfVotes)
    Assert.Equal(1, res.[1].numberOfVotes)

[<Fact>]
let ``getAggregatedVoteWhereCandidateIsOnPosiotion`` () =
    let position = 1
    let aggregatedVoteList = [
        {AggregatedVote.ballot = [ 1; 2 ]; numberOfVotes = 1};
        {AggregatedVote.ballot = [ 1; 2 ]; numberOfVotes = 2};
        {AggregatedVote.ballot = [ 3; 4 ]; numberOfVotes = 3}
    ]
    let candidateId = 1
    let res = getAggregatedVoteWhereCandidateIsOnPosiotion position aggregatedVoteList candidateId
    
    Assert.Equal(2, List.length res)
    Assert.Equal(1, res.[0].numberOfVotes)
    Assert.Equal(2, res.[1].numberOfVotes)

[<Fact>]
let ``getNextCandidateId`` () =
    let ballot = [4;5;6]

    let res1 = getNextCandidateId ballot 1
    Assert.True(res1.IsNone)

    let res4 = getNextCandidateId ballot 4
    Assert.Equal(5, res4.Value)

    let res5 = getNextCandidateId ballot 5
    Assert.Equal(6, res5.Value)

    let res6 = getNextCandidateId ballot 6
    Assert.True(res6.IsNone)

[<Fact>]
let ``getCandidateTotalVotes`` () =
    let pollResultItemList = [
        {candidateId = 1; numberOfVotes = 4; elected = true};
        {candidateId = 2; numberOfVotes = 5; elected = true};
        {candidateId = 3; numberOfVotes = 6; elected = true};
     ]
    let res = getCandidateTotalVotes pollResultItemList 2
    Assert.Equal(5, res)

[<Fact>]
let ``calculateSurplusVotesToAdd`` () =
    let votesForTheNextPreference = 8
    let totalWinnerVotes = 12
    let surplusVotes = 6
    let res = calculateSurplusVotesToAdd votesForTheNextPreference totalWinnerVotes surplusVotes
    Assert.Equal(4, res)

[<Fact>]
let ``addNumberOfVotesToResult add`` () =
    let pollResultItemList = [
        {candidateId = 1; numberOfVotes = 4; elected = true};
        {candidateId = 2; numberOfVotes = 5; elected = true};
        {candidateId = 3; numberOfVotes = 6; elected = true};
    ]
    let candidateId = 2
    let votesToAdd = 10
    let res = addNumberOfVotesToResult pollResultItemList candidateId votesToAdd
    Assert.Equal(4, res.[0].numberOfVotes)
    Assert.Equal(15, res.[1].numberOfVotes)
    Assert.Equal(6, res.[2].numberOfVotes)

[<Fact>]
let ``addNumberOfVotesToResult substract`` () =
    let pollResultItemList = [
        {candidateId = 1; numberOfVotes = 4; elected = true};
        {candidateId = 2; numberOfVotes = 5; elected = true};
        {candidateId = 3; numberOfVotes = 6; elected = true};
    ]
    let candidateId = 2
    let votesToAdd = -10
    let res = addNumberOfVotesToResult pollResultItemList candidateId votesToAdd
    Assert.Equal(4, res.[0].numberOfVotes)
    Assert.Equal(-5, res.[1].numberOfVotes)
    Assert.Equal(6, res.[2].numberOfVotes)

// https://en.wikipedia.org/wiki/Counting_single_transferable_votes
[<Fact>]
let ``addOneSurplus`` () =
    let aggregatedVoteList = [
        {AggregatedVote.ballot = [ 1; 2; 3; 4 ]; numberOfVotes = 16};
        {AggregatedVote.ballot = [ 1; 3; 2; 4 ]; numberOfVotes = 24};
        {AggregatedVote.ballot = [ 4; 1; 2; 3 ]; numberOfVotes = 17}
    ]
    let pollResultItemList = [
        {candidateId = 1; numberOfVotes = 40; elected = true};
        {candidateId = 2; numberOfVotes = 0; elected = false};
        {candidateId = 3; numberOfVotes = 0; elected = false};
        {candidateId = 4; numberOfVotes = 17; elected = false};
    ]
    let surplus = (1, 20)
    let res = addOneSurplus aggregatedVoteList pollResultItemList surplus
    let expected = [
        {candidateId = 1; numberOfVotes = 20; elected = true};
        {candidateId = 2; numberOfVotes = 8; elected = false};
        {candidateId = 3; numberOfVotes = 12; elected = false};
        {candidateId = 4; numberOfVotes = 17; elected = false};
    ]

    Assert.Equal(expected.[0], res.[0])
    Assert.Equal(expected.[1], res.[1])
    Assert.Equal(expected.[2], res.[2])

// https://en.wikipedia.org/wiki/Counting_single_transferable_votes
[<Fact>]
let ``mainCaluclation`` () =
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

    Assert.True(true)
