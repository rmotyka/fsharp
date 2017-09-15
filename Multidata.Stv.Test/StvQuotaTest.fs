module Multidata.Stv.Test.StvQuotaTest

open Xunit
open Multidata.Stv.StvQuota

[<Fact>]
let ``droop quota`` () =
    let res = calculateDroopQuota 2 100
    Assert.Equal(34, res)