﻿open System
open System.Configuration
open System.Data.SqlClient
open System.IO
open System.Reflection
open System.Text.RegularExpressions

open Dapper

let private getSqlDirectory =
    let asm = Assembly.GetExecutingAssembly()
    let uri = new Uri(asm.CodeBase)
    let asmDir = uri.LocalPath |> Path.GetDirectoryName
    let relativeDir = ConfigurationManager.AppSettings.["sqlDirectoryRelativePath"]
    Path.Combine(asmDir, relativeDir)

let private masterConnectionString = 
    ConfigurationManager.ConnectionStrings.["master"].ConnectionString
        
let private djambiConnectionString = 
    ConfigurationManager.ConnectionStrings.["djambi"].ConnectionString

let private executeCommand (cnStr : string)(command : string) : unit =
    use cn = new SqlConnection(cnStr)
    cn.Execute(command) |> ignore
    
let private dropAndCreateDb() : unit =
    printfn "Dropping and creating database"
    let sql = "IF EXISTS(SELECT * FROM sys.databases WHERE name='Djambi')
               DROP DATABASE Djambi;
               CREATE DATABASE Djambi;"
    executeCommand masterConnectionString sql

let private loadFile (relativePath : string) : unit =
    printfn "Loading %s" relativePath
    let absolutePath = Path.Combine(getSqlDirectory, relativePath)
    let text = File.ReadAllText(absolutePath)
    let commands = Regex.Split(text, "\s+GO")
                   |> Seq.filter (String.IsNullOrEmpty >> not)
    for c in commands do    
        executeCommand djambiConnectionString c

let getFilesInOrder : string seq =
    seq {
        //Order is very important for foreign keys
        let tables = [
            "Roles"
            "GameStatuses"
            "VirtualPlayerNames"

            "Users"
            "Games"
            "Players"
            "Turns"
            "Messages"        
        ]

        yield! tables |> Seq.map (fun name -> sprintf "Tables\\dbo.%s.sql" name)

        let getFiles folder =
            Path.Combine(getSqlDirectory, folder)
            |> Directory.EnumerateFiles
            |> Seq.map (fun path -> Path.Combine(folder, Path.GetFileName(path)))

        let folders = [
            "Security\\Schemas"
            "Stored Procedures"
            "Data"
        ]
 
        yield! folders |> Seq.collect getFiles
    }

[<EntryPoint>]
let main argv =
    dropAndCreateDb()
    for f in getFilesInOrder do
        loadFile f
    
    printfn "Done"
    Console.Read() |> ignore
    0