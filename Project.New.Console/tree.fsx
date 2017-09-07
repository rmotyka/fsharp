type Tree =
  | Branch of string * Tree list
  | Leaf of string

let t2 = Branch ("a", [Branch ("b", [Leaf "c"; Leaf "d"]); Branch ("e", [Leaf "f"; Leaf "g"])])

let rec checkstuff tree =
    match tree with
    | Leaf x -> 
        printf "Leaf %O " x
        true
    | Branch (node, children) ->
        printf "Branch %O " node
        List.fold ( || ) false (List.map checkstuff children)