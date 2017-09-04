namespace Project.New.Library


module Drzewko =
    type Item = {  Id : int;  Parent : int; }
    
    let ListOfItems : Item list = 
        [ 
            { Id = 1; Parent = 0 };
            { Id = 2; Parent = 1 };
            { Id = 3; Parent = 2 };
            { Id = 4; Parent = 3 }
        ]