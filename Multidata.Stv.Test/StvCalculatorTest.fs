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
    let res = droopQuota 2 100
    Assert.Equal(33, res)

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