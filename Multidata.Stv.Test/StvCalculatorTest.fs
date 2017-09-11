module Multidata.Stv.Test.StvCalculator

open System
open Xunit

[<Fact>]
let ``My test`` () =
    Assert.True(true)

[<Fact>]
let ``Droop`` () =
    let res = Multidata.Stv.StvCalculator.droopQuota 2 100
    Assert.Equal(33, res)