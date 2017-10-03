module Multidata.Stv.StvCalculator

open StvModels
open Multidata.Stv.StvQuota

let aggregateVotes (voteList: Ballot list) =
    List.countBy (id) voteList 
     |> List.map (fun (b, c) -> {ballot = b; numberOfVotes = c})

let getBallotFirstCandidateId (preference: int) (ballot: Ballot) = 
   List.item (preference - 1) ballot

let sumVotes (preference: int) (aggregatedVoteList: AggregatedVote list) =
    List.map (fun x -> (getBallotFirstCandidateId 1 x.ballot, x.numberOfVotes)) aggregatedVoteList // simplify the structure
    |> List.groupBy (fun (a, b) -> a) // aggregate by candidateId
    |> List.map (fun (key , values) -> (key, List.sumBy snd values)) // sum votes for candidate - I don't like: List.sumBy snd values

let voteSumToPollResult droopQuota votesSum = 
    let filteredItems = List.filter (fun (c, v) -> v >= droopQuota) votesSum
    List.map (fun (c, v) -> {candidateId = c; numberOfVotes = v; elected = true}) filteredItems

let isPollFinished numberOfSeats pollResultItemList =
    List.filter (fun x -> x.elected) pollResultItemList
    |> List.length >= numberOfSeats

let getSurplus droopQuota (pollResultItemList: PollResultItem list) = 
    pollResultItemList
    |> List.filter (fun x -> x.numberOfVotes > droopQuota)
    |> List.map (fun x -> (x.candidateId, x.numberOfVotes - droopQuota))

let getAggregatedVoteWhereCandidateIsOnPosiotion position aggregatedVoteList candidateId = 
    List.filter (fun x -> List.item (position - 1) x.ballot = candidateId) aggregatedVoteList

let getNextCandidateId ballot candidateId = 
    let maybeIndex = List.tryFindIndex (fun x -> x = candidateId) ballot
    match maybeIndex with
    | Some i -> List.tryItem (i + 1) ballot
    | None -> None

let getCandidateTotalVotes pollResultItemList candidateId = 
    let winnersResult = List.find (fun x -> x.candidateId = candidateId) pollResultItemList
    winnersResult.numberOfVotes

let calculateSurplusVotesToAdd votesForTheNextPreference totalWinnerVotes surplusVotes = 
    (int)(((float)votesForTheNextPreference / (float)totalWinnerVotes) * (float)surplusVotes)

// let addNumberOfVotesToResult pollResultItemList candidateId votesToAdd = 
//     List.map (fun x -> 
//     if x.candidateId = candidateId then
//         {candidateId = x.candidateId; numberOfVotes = x.numberOfVotes + votesToAdd; elected = false}
//     else
//         x
//     ) pollResultItemList

// // TODO: test
// let addOneSurplus aggregatedVoteList pollResultItemList surplus = 
//     let (winnerCandidateId, surplusNumberOfVotes) = surplus
//     let winnerTotalVotes = getCandidateTotalVotes pollResultItemList winnerCandidateId
//     let position = 1 // TODO: get from arguments
//     let aggregateVotes = getAggregatedVoteWhereCandidateIsOnPosiotion position aggregatedVoteList winnerCandidateId
//     List.fold (fun acc aggregateVote -> 
//         let maybeNextCandidateId = getNextCandidateId aggregateVote.ballot winnerCandidateId
//         match maybeNextCandidateId with
//         | Some nextCandidateId -> 
//             let votesToAdd = calculateSurplusVotesToAdd aggregateVote.numberOfVotes winnerTotalVotes surplusNumberOfVotes
//             let tempPollResult = addNumberOfVotesToResult acc nextCandidateId votesToAdd
//             addNumberOfVotesToResult tempPollResult nextCandidateId -votesToAdd
//         | None -> acc
//     ) pollResultItemList aggregateVotes

// // TODO: test
// let addSurplus aggregatedVoteList pollResultItemList surplusList =
//     List.fold (fun acc surplus -> addOneSurplus aggregatedVoteList acc surplus) pollResultItemList surplusList

// let rec iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResultItemList =
//     let surplusList = getSurplus droopQuota pollResultItemList
//     let pollResultWithSurplus = addSurplus aggregatedVoteList pollResultItemList surplusList

//     let numberResults = List.length pollResultItemList
//     match numberResults with
//     //| 0 -> calculateFirstPoll 
//     | numberOfSeats -> pollResultItemList
//     | _ -> iterationLoop numberOfSeats droopQuota aggregatedVoteList pollResultItemList

let getFirstRow aggregatedVotes = 
    aggregatedVotes |> List.map(fun x -> (x.ballot.[0], x.numberOfVotes))

let aggregageByUser (firstRow: (int * int) list) = 
    firstRow
    |> List.groupBy (fun (a, b) -> a) // aggregate by candidateId
    |> List.map (fun (key , values) -> (key, List.sumBy snd values)) // sum votes for candidate - I don't like: List.sumBy snd values

let getWinner quota firstRow =
    firstRow 
    |> List.tryFind(fun x -> snd x >= quota)

let removeCandidateFromBalot votesSurplus winner aggregatedVote = 
    let candidateId = fst winner
    let newVotes = calculateSurplusVotesToAdd aggregatedVote.numberOfVotes (snd winner) votesSurplus
    {
        numberOfVotes = newVotes
        ballot = List.filter (fun x -> x <> candidateId) aggregatedVote.ballot
    }

let removeCandidate aggregatedVotes quota winner = 
    let votesSurplas = (snd winner) - quota
    aggregatedVotes |> List.map(fun x -> removeCandidateFromBalot votesSurplas winner x)

let getLooser firstRow =
    firstRow |> List.minBy(snd)


let rec iterationLoop2 numberOfSeats allWinners droopQuota aggregatedVotes :int list =
    let firstRow = getFirstRow aggregatedVotes
    let fistRowSum = aggregageByUser firstRow
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
            // loop
            iterationLoop2 numberOfSeats allWinners droopQuota aggregatedVotes
    | None ->
        // get last candidate
        let looser = getLooser fistRowSum
        let aggregatedVotes = removeCandidate aggregatedVotes 0 looser
        // loop
        iterationLoop2 numberOfSeats allWinners droopQuota aggregatedVotes


// Only valid and sorted ballots
let mainCalculation (poll: Poll) (voteList: Ballot list) =
    let totalValidPoll = List.length voteList
    let droopQuota = calculateDroopQuota poll.numberOfSeats totalValidPoll

    let aggregatedVotes = aggregateVotes voteList
    let winners = iterationLoop2 poll.numberOfSeats [] droopQuota aggregatedVotes
    winners
    
    
    // // first round
    // let votesSum = sumVotes 1 aggregatedVotes
    // let pollResult = voteSumToPollResult droopQuota votesSum
    // let pollFinished = isPollFinished poll.numberOfSeats pollResult
    // if pollFinished then
    //     pollResult
    // else
    //     iterationLoop poll.numberOfSeats droopQuota aggregatedVotes pollResult
