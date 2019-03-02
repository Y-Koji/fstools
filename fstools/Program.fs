open System.Text.RegularExpressions
open System.IO
open System

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

let mutable CurrentDirectory = Environment.CurrentDirectory;

let replaceFileNames matchExpression (replaceExpression: string) : unit =
    Directory.GetFiles(CurrentDirectory) 
    |> Array.filter (fun x -> Regex.IsMatch(x, matchExpression))
    |> Array.iter (fun x -> 
        let fileName = x
        let newFileName = Regex.Replace(x, matchExpression, replaceExpression)
        File.Move(fileName, newFileName))

let rec findOption (name: string) (xs: string[]) (defaultValue: Option<string>) =
    if xs.Length < 2 then defaultValue else
    match xs.[0..1] with
        | [| name'; value; |] when name' = name -> Some value
        | _ -> if 2 < xs.Length then findOption name xs.[1..] defaultValue else defaultValue

[<EntryPoint>]
let main argv = 
    CurrentDirectory <- (findOption "-current-directory" argv (Some Environment.CurrentDirectory)).Value
    let matchExpression = findOption "-match" argv None
    let replaceExpression = findOption "-replace" argv None
    match matchExpression, replaceExpression with
        | None, None -> ()
        | Some matchExpression, Some replaceExpression -> replaceFileNames matchExpression replaceExpression
        | _ ->
            printfn "fstools - F#で適当に作るツール"
            printfn "Usage: "
            printfn "[正規表現でファイル名一括変換]"
            printfn "  -match {fileNameExpression} -replace {replaceExpression}"
            printfn "    {fileName}にマッチする全ファイルを{replaceExpression}置換します．"
            
    0 // return an integer exit code
