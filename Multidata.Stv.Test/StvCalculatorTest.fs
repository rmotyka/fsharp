module Multidata.Stv.Test.StvCalculator

open System
open Xunit
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
    let candidates = [{1; "Smith"}; {2;"Gordon"}]
    let poll = {numberOfSeats = 12; candidates = candidates}
    let voteList = [[{1;1}]]
    let res = mainCaluclation poll voteList

    Assert.Equal()