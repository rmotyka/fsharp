/// https://goc.vivint.com/problems/flips

type Result = {
    Score: int
    FippedCollumns: int list
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

let mainCalc input numberOfCollumns =
    let currentScore = calcScore input
    let currentResult = { Score = currentScore; FippedCollumns = [] }
    let columnsCombinations = allCombinations [0..numberOfCollumns-1]
        
        

    currentResult
