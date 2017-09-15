module Multidata.Stv.StvQuota

let calculateDroopQuota numberOfSeats totalValidPoll = 
    (float totalValidPoll + float 1) / (float numberOfSeats + float 1) |> ceil |> int