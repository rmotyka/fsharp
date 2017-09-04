let add42 = (+) 42    // partial application
add42 1
add42 3

// create a new list by applying the add42 function 
// to each element
[1;2;3] |> List.map add42 

let twoIsLessThan = (<) 2   // partial application
twoIsLessThan 1
twoIsLessThan 3

// filter each element with the twoIsLessThan function
[1;2;3] |> List.filter twoIsLessThan 