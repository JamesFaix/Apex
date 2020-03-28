module Apex.Api.Db.Model

open System
open Apex.Api.Model
open Apex.Api.Enums

[<CLIMutable>]
type UserSqlModel =
    {
        userId : int
        name : string
        password : string
        failedLoginAttempts : byte
        lastFailedLoginAttemptOn : DateTime Nullable
    }

[<CLIMutable>]
type SessionSqlModel =
    {
        sessionId : int
        userId : int
        userName : string
        token : string
        createdOn : DateTime
        expiresOn : DateTime
    }

[<CLIMutable>]
type PlayerSqlModel =
    {
        playerId : int
        gameId : int
        userId : int Nullable
        name : string
        playerKindId : PlayerKind
        playerStatusId : PlayerStatus
        colorId : byte Nullable
        startingRegion : byte Nullable
        startingTurnNumber : byte Nullable
    }

[<CLIMutable>]
type GameSqlModel =
    {
        gameId : int
        createdOn : DateTime
        createdByUserId : int
        createdByUserName : string
        gameStatusId : GameStatus
        //Game parameters
        description : string
        regionCount : int
        isPublic : bool
        allowGuests : bool
        //State
        turnCycleJson : string
        piecesJson : string
        currentTurnJson : string
    }

[<CLIMutable>]
type SearchGameSqlModel =
    {
        gameId : int
        createdOn : DateTime
        createdByUserId : int
        createdByUserName : string
        containsMe : bool
        gameStatusId : GameStatus
        //Game parameters
        description : string
        regionCount : int
        isPublic : bool
        allowGuests : bool
        //State
        lastEventOn : DateTime
        playerCount : int
        //TODO: Add current player name
    }

[<CLIMutable>]
type EventSqlModel =
    {
        eventId : int
        gameId : int
        createdByUserId : int
        createdByUserName : string
        actingPlayerId : int Nullable
        createdOn : DateTime
        eventKindId : EventKind
        effectsJson : string
    }

[<CLIMutable>]
type SnapshotSqlModel =
    {
        snapshotId : int
        gameId : int
        createdByUserId : int
        createdByUserName : string
        createdOn : DateTime
        description : string
        snapshotJson : string
    }

[<CLIMutable>]
type SnapshotJson =
    {
        game : Game
        history : Event list
    }