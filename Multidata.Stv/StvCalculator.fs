module Multidata.Stv.StvCalculator

//open Microsoft.FSharp.Math

let droopQuota seats totalValidPoll = 
    (float totalValidPoll + float 1) / (float seats + float 1) |> floor |> int