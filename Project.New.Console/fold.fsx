let numbers = [1..10]

let mult1 factor numbers =
    List.fold (fun acc x -> acc @ [x * factor]) [] numbers

let where predicate numbers = 
    List.fold (fun acc x -> if predicate x then acc @ [x] else acc) [] numbers

let pred1 elem =
    if elem > 3 then true else false
    
/////////// foldBack
List.fold (fun acc x -> printf "%d " x ) () [1..10]
List.foldBack (fun x acc -> printf "%d " x ) [1..10] ()

////////// fold2

List.fold2 (fun acc x y -> printf "%d%s " x y) () [1..3] ["a"; "b"; "c"]
List.foldBack2 (fun y x acc -> printf "%d%s " x y)  ["a"; "b"; "c"] [1..3] ()