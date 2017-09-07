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

List.fold2 (fun acc x y -> printf "%d%O " x y) () [1..3] ['a'..'c']
List.foldBack2 (fun y x acc -> printf "%d%O " x y)  ['a'..'c'] [1..3] ()

///////// append

let list1 = List.append [1; 2] [3; 4]
let list2 = List.concat [ [5; 6 ]; [7; 8] ]
let list3 = list1 @ list2

// average

List.map (float) [0; 1; 1; 2] |> List.average 


// choose

List.choose (fun x -> x) [ Some(2); None;  Some(3);  None; Some(4)]