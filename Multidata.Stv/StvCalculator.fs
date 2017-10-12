module Multidata.Stv.StvCalculator

open StvModels
open Multidata.Stv.StvQuota

let aggregateVotes (voteList: Ballot list) =
    List.countBy (id) voteList 
     |> List.map (fun (b, c) -> {ballot = b; numberOfVotes = c})

let calculateSurplusVotesToAdd votesForTheNextPreference totalWinnerVotes surplusVotes = 
    (int)(((float)votesForTheNextPreference / (float)totalWinnerVotes) * (float)surplusVotes)

let getFirstRow aggregatedVotes = 
    aggregatedVotes |> List.map(fun x -> (x.ballot.[0], x.numberOfVotes))

let aggregateByCandidate (firstRow: (int * int) list) = 
    firstRow
    |> List.groupBy (fun (a, b) -> a) // aggregate by candidateId
    |> List.map (fun (key , values) -> (key, List.sumBy snd values)) // sum votes for candidate - I don't like: List.sumBy snd values

let getWinner quota firstRow =
    firstRow 
    |> List.tryFind(fun x -> snd x >= quota)

let removeCandidateFromBallot votesSurplus winner aggregatedVote = 
    let candidateId = fst winner
    let newVotes = 
        if aggregatedVote.ballot.[0] = candidateId then
            calculateSurplusVotesToAdd aggregatedVote.numberOfVotes (snd winner) votesSurplus
        else
            aggregatedVote.numberOfVotes
    {
        numberOfVotes = newVotes
        ballot = List.filter (fun x -> x <> candidateId) aggregatedVote.ballot
    }

let removeCandidate aggregatedVotes quota winner = 
    let votesSurplus = (snd winner) - quota
    aggregatedVotes |> List.map(fun x -> removeCandidateFromBallot votesSurplus winner x)

let getLooser firstRow =
    firstRow |> List.minBy(snd)

let rec iterationLoop numberOfSeats allWinners droopQuota aggregatedVotes =
    printfn "aggregatedVotes %A" aggregatedVotes
    let firstRow = getFirstRow aggregatedVotes
    let fistRowSum = aggregateByCandidate firstRow
    let maybeWinner = getWinner droopQuota fistRowSum
    match maybeWinner with
    | Some winner ->
        let allWinners = (fst winner) :: allWinners
        if 
            allWinners.Length >= numberOfSeats
        then 
            allWinners
        else
            let aggregatedVotes = removeCandidate aggregatedVotes droopQuota winner
            iterationLoop numberOfSeats allWinners droopQuota aggregatedVotes
    | None ->
        let looser = getLooser fistRowSum
        let aggregatedVotes = removeCandidate aggregatedVotes 0 looser
        iterationLoop numberOfSeats allWinners droopQuota aggregatedVotes

// Only valid and sorted ballots
let mainCalculation (poll: Poll) (voteList: Ballot list) =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll

    printfn "Quota: %d" droopQuota

    let aggregatedVotes = aggregateVotes voteList
    let winnersCandidateIdList = iterationLoop poll.numberOfSeats [] droopQuota aggregatedVotes
    poll.candidates |> List.filter(fun x -> List.contains x.candidateId winnersCandidateIdList)
    