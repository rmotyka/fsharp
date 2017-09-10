module Tests

open System
open Xunit
open Project.New.Library

[<Fact>]
let ``My test`` () =
    Assert.True(true)


[<Fact>]
let ``My test 1`` () =
    let odp = Say.hello "x"
    Assert.Equal("Hello x", odp)
    
