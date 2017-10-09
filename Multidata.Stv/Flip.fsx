/// https://goc.vivint.com/problems/flips

type Result = {
    Score: int
    FlippedColumns: int list
}

let input = [
    [1; 0; 0; 1; 0]
    [1; 0; 0; 1; 0]
    [1; 0; 1; 0; 0]
    [0; 1; 1; 0; 1]
    [1; 0; 0; 1; 0]
]

let rowScore row =
    if List.forall (fun x -> x = 1) row ||List.forall (fun x -> x = 0) row then
       1 else 0    

let calcScore input = 
    input 
    |> List.fold (fun acc x -> acc + rowScore x) 0

let allCombinations lst =
    let rec comb accLst elemLst =
        match elemLst with
        | h::t ->
            let next = [h]::List.map (fun el -> h::el) accLst @ accLst
            comb next t
        | _ -> accLst
    comb [] lst

let flip x =
    if x = 1 then 0 else 1

let flipColumns combination row = 
    row
    |> List.mapi (fun i x -> if List.contains i combination then flip x else x)

let combine input combination = 
    input
    |> List.map (fun row -> flipColumns combination row)

let setBetterCombinations input currentResult combination =
    let changedInput = combine input combination
    let currentScore = calcScore changedInput
    if currentScore > currentResult.Score 
        || (currentScore = currentResult.Score && currentResult.FlippedColumns.Length > combination.Length) then
            { Score = currentScore; FlippedColumns = combination }
    else
        currentResult

let mainCalc input numberOfColumns =
    let initalScore = calcScore input
    let initialResult = { Score = initalScore; FlippedColumns = [] }
    let modificator = setBetterCombinations input
    let result = 
        [0..numberOfColumns-1] 
        |> allCombinations 
        |> List.fold modificator initialResult
    { Score = result.Score; FlippedColumns = List.sort result.FlippedColumns }
