[<AutoOpen>]
module Apex.Api.Model.GameRequestModel

open System
open System.ComponentModel
open Apex.Api.Enums

type CreatePlayerRequest =
    {
        kind : PlayerKind
        userId : int option
        name : string option
    }

module CreatePlayerRequest =

    let user (user : User) : CreatePlayerRequest =
        {
            kind = PlayerKind.User
            userId = Some user.id
            name = None
        }

    let guest (userId : int, name : string) : CreatePlayerRequest =
        {
            kind = PlayerKind.Guest
            userId = Some userId
            name = Some name
        }

    let neutral (name : string) : CreatePlayerRequest =
        {
            kind = PlayerKind.Neutral
            userId = None
            name = Some name
        }

type PlayerStatusChangeRequest =
    {
        gameId : int
        playerId : int
        status : PlayerStatus
    }

type SelectionRequest =
    {
        cellId : int
    }

type EventsQuery =
    {
        maxResults : int option
        direction : ListSortDirection
        thresholdTime : DateTime option
        thresholdEventId : int option
    }

module EventsQuery =
    let empty : EventsQuery =
        {
            maxResults = None
            direction = ListSortDirection.Ascending
            thresholdTime = None
            thresholdEventId = None
        }

//---Internal requests

type UpdateFailedLoginsRequest =
    {
        userId : int
        failedLoginAttempts : int
        lastFailedLoginAttemptOn : DateTime option
    }

module UpdateFailedLoginsRequest =
    let reset (userId : int) =
        {
            userId = userId
            failedLoginAttempts = 0
            lastFailedLoginAttemptOn = None
        }

    let increment (userId : int, attempts : int) =
        {
            userId = userId
            failedLoginAttempts = attempts
            lastFailedLoginAttemptOn = Some DateTime.UtcNow
        }

type CreateSessionRequest =
    {
        userId : int
        token : string
        expiresOn : DateTime
    }

type SessionQuery =
    {
        sessionId : int option
        token : string option
        userId : int option
    }

module SessionQuery =
    let byId (sessionId : int) =
        {
            sessionId = Some sessionId
            token = None
            userId = None
        }

    let byToken (token : string) =
        {
            sessionId = None
            token = Some token
            userId = None
        }

    let byUserId (userId : int) =
        {
            sessionId = None
            token = None
            userId = Some userId
        }

type UpdateGameStateRequest =
    {
        gameId : int
        status : GameStatus
        pieces : Piece list
        currentTurn : Turn option
        turnCycle : int list
    }

type SetPlayerStartConditionsRequest =
    {
        playerId : int
        colorId : int
        startingRegion : int
        startingTurnNumber : int option
    }

type CreateGameRequest =
    {
        parameters : GameParameters
        createdByUserId : int
    }