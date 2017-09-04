namespace Project.New.Library

open System
open System.Globalization

module Say =
    let rev str =
        let si = StringInfo(str)
        let teArr = Array.init si.LengthInTextElements (fun i -> si.SubstringByTextElements(i,1))
        Array.Reverse(teArr) //in-place reversal better performance than Array.rev
        String.Join("", teArr)

    let hello name =
        "Hello " + rev name
