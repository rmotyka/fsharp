module Multidata.Stv.StvCalculator

open StvModels

let droopQuota seats totalValidPoll = 
    (float totalValidPoll + float 1) / (float seats + float 1) |> floor |> int

//--------------------------------

// Only valid votes
let mainCaluclation (poll: Poll) (voteList: VoteList) : PollResult =

    let pollResult = {items = [{candidateId = 1; numberOfVotes = 100; elected = true}]}
    pollResult
