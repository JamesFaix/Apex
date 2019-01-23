﻿module Djambi.Api.Db.Repositories.EventRepository

open System.Data
open System.Linq
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Model
open Djambi.Api.Db
        
let private getCommands (oldGame : Game, newGame : Game, transaction : IDbTransaction) : CommandDefinition seq = 
    let commands = new ArrayList<CommandDefinition>()

    //remove players
    let removedPlayers = 
        oldGame.players 
        |> Seq.filter (fun oldP -> 
            newGame.players 
            |> (not << Seq.exists (fun newP -> oldP.id = newP.id )))

    for p in removedPlayers do
        let cmd = GameRepository.getRemovePlayerCommand p.id
                  |> CommandDefinition.withTransaction transaction
        commands.Add(cmd)
        
    //add players
    let addedPlayers = 
        newGame.players
        |> Seq.filter (fun newP ->
            oldGame.players
            |> (not << Seq.exists (fun oldP -> oldP.id = newP.id)))

    for p in addedPlayers do
        let playerRequest = 
            {
                kind = p.kind
                userId = p.userId
                name = if p.kind = PlayerKind.User then None else Some p.name
            }       
        let cmd = GameRepository.getAddPlayerCommand(oldGame.id, playerRequest)
                  |> CommandDefinition.withTransaction transaction
        commands.Add(cmd)
    //update players
    let modifiedPlayers =
        Enumerable.Join(
            oldGame.players, newGame.players,
            (fun p -> p.id), (fun p -> p.id),
            (fun _ newP -> newP)
        )

    for p in modifiedPlayers do
        let cmd = GameRepository.getUpdatePlayerCommand p
                  |> CommandDefinition.withTransaction transaction
        commands.Add(cmd)
 
    //update game
    if oldGame.parameters <> newGame.parameters
        || oldGame.currentTurn <> newGame.currentTurn
        || oldGame.pieces <> newGame.pieces
        || oldGame.turnCycle <> newGame.turnCycle
        || oldGame.status <> newGame.status
    then
        let cmd = GameRepository.getUpdateGameCommand newGame
                  |> CommandDefinition.withTransaction transaction
        commands.Add(cmd) 
    else ()

    commands |> Enumerable.AsEnumerable

let persistEvent (oldGame : Game, newGame : Game) : Game AsyncHttpResult =
    task {
        use conn = SqlUtility.getConnection()
        use tran = conn.BeginTransaction()
        let commands = getCommands (oldGame, newGame, tran)   
    
        try 
            for cmd in commands do
                let _ = conn.Execute(cmd)
                ()
            tran.Commit()
            return Ok ()
        with 
        | _ as ex -> return Error <| (SqlUtility.catchSqlException ex "Effect")
    }
    |> AsyncHttpResult.thenBindAsync (fun _ -> GameRepository.getGame newGame.id)
    