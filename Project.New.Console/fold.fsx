let numbers = [1..10]

let mult1 factor numbers =
    List.fold (fun acc x -> acc @ [x * factor]) [] numbers

let where predicate numbers = 
    List.fold (fun acc x -> if predicate x then acc @ [x] else acc) [] numbers

let pred1 elem =
    if elem > 3 then true else false
