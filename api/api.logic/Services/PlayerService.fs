﻿module Djambi.Api.Logic.Services.PlayerService

open System
open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model
    
let getAddPlayerEvent (game : Game, request : CreatePlayerRequest) (session : Session) : CreateEventRequest HttpResult =
    let self = session.user
    if game.status <> GameStatus.Pending
    then Error <| HttpException(400, "Can only add players to pending games.")
    elif request.name.IsSome 
        && game.players |> List.exists (fun p -> String.Equals(p.name, request.name.Value, StringComparison.OrdinalIgnoreCase))
    then Error <| HttpException(409, "A player with that name already exists.")
    elif game.players.Length >= game.parameters.regionCount
    then Error <| HttpException(400, "Max player count reached.")
      else
        match request.kind with
        | PlayerKind.User ->
            if request.userId.IsNone
            then Error <| HttpException(400, "UserID must be provided when adding a user player.")
            elif request.name.IsSome
            then Error <| HttpException(400, "Cannot provide name when adding a user player.")
            elif game.players |> List.exists (fun p -> p.kind = PlayerKind.User && p.userId = request.userId)
            then Error <| HttpException(409, "User is already a player.")
            elif not self.isAdmin && request.userId.Value <> self.id
            then Error <| HttpException(403, "Cannot add other users to a game.")
            else Ok ()

        | PlayerKind.Guest ->
            if not game.parameters.allowGuests
            then Error <| HttpException(400, "Game does not allow guest players.")
            elif request.userId.IsNone
            then Error <| HttpException(400, "UserID must be provided when adding a guest player.")
            elif request.name.IsNone
            then Error <| HttpException(400, "Must provide name when adding a guest player.")
            elif not self.isAdmin && request.userId.Value <> self.id
            then Error <| HttpException(403, "Cannot add guests for other users to a game.")
            else Ok ()

        | PlayerKind.Neutral ->
            Error <| HttpException(400, "Cannot directly add neutral players to a game.")
    |> Result.map (fun _ -> 
        {
            kind = EventKind.PlayerJoined
            effects = [ PlayerAddedEffect.fromRequest request ]
            createdByUserId = self.id
        }
    )

let getRemovePlayerEvent (game : Game, playerId : int) (session : Session) : CreateEventRequest HttpResult =
    let self = session.user
    match game.status with
    | Aborted | AbortedWhilePending | Finished -> 
        Error <| HttpException(400, "Cannot remove players from finished or aborted games.")
    | _ ->
        match game.players |> List.tryFind (fun p -> p.id = playerId) with
        | None -> Error <| HttpException(404, "Player not found.")
        | Some player ->
            match player.userId with
            | None -> Error <| HttpException(400, "Cannot remove neutral players from game.")
            | Some x ->
                if not <| (self.isAdmin
                    || game.createdByUserId = self.id
                    || x = self.id)
                then Error <| HttpException(403, "Cannot remove other users from game.")        
                else 
                    let effects = new ArrayList<Effect>()

                    let playerIdsToRemove =
                        match player.kind with 
                        | User -> 
                            game.players 
                            |> List.filter (fun p -> p.userId = player.userId) 
                            |> List.map (fun p -> p.id)
                        | Guest -> [playerId]
                        | _ -> List.empty //Already eliminated this case in validation above

                    for pId in playerIdsToRemove do
                        effects.Add(Effect.PlayerRemoved { playerId = pId })

                    //Cancel game if Pending and creator quit
                    if game.status = GameStatus.Pending
                        && game.createdByUserId = player.userId.Value
                        && player.kind = PlayerKind.User
                    then 
                        effects.Add(Effect.GameStatusChanged { oldValue = GameStatus.Pending; newValue = GameStatus.AbortedWhilePending })
                    else ()

                    let kind = 
                        if player.userId = Some self.id
                        then EventKind.PlayerQuit
                        else EventKind.PlayerEjected

                    {
                        kind = kind
                        effects = effects |> Seq.toList
                        createdByUserId = self.id
                    }
                    |> Ok

let fillEmptyPlayerSlots (game : Game) : Effect list AsyncHttpResult =
    let missingPlayerCount = game.parameters.regionCount - game.players.Length

    let getNeutralPlayerNamesToUse (possibleNames : string list) =
        Enumerable.Except(
            possibleNames,
            game.players |> Seq.map (fun p -> p.name),
            StringComparer.OrdinalIgnoreCase)
        |> List.ofSeq
        |> List.shuffle
        |> Seq.take missingPlayerCount

    if missingPlayerCount = 0
    then okTask List.empty
    else
        GameRepository.getNeutralPlayerNames()
        |> thenMap getNeutralPlayerNamesToUse
        |> thenMap (Seq.map (fun name -> PlayerAddedEffect.fromRequest <| CreatePlayerRequest.neutral name ))    
        |> thenMap Seq.toList