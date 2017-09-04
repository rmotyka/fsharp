﻿// Learn more about F# at http://fsharp.org
open System
open Project.New.Library

let rec allParents (lista : Drzewko.Item list) (id : int) =
    [
        let currentRecord = List.tryFind (fun (x : Drzewko.Item) -> x.Id = id) lista
        match currentRecord with
        | Some x -> 
            yield! allParents lista x.Parent
            yield x
        | None -> ()
    ]

let pokazListe lista = 
    lista |> List.iter (fun x -> printfn "%A" x)

let add1 x =
    x + 1

let multiply2 x =
    x * 2
    
let oblicz x =
    let w1 = add1 x
    let w2 = add1 w1
    let w3 = multiply2 w2
    w3

let inneAdd = add1

let combine f g x =
    let w1 = g x
    f w1

let combineBezArg = 


[<EntryPoint>]
let main argv =
    let items = Drzewko.ListOfItems
    // pokazListe items

    let parents = allParents items 4
    pokazListe parents
    
    let t = Say.hello "Roman"
    printfn "Hello World from F#! %A" t
    0 // return an integer exit code


