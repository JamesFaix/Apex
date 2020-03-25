﻿namespace Apex.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations

type PlayerKindDto =
    | User = 1
    | Guest = 2
    | Neutral = 3

type PlayerStatusDto = 
    | Pending = 1
    | Alive = 2
    | Eliminated = 3
    | Conceded = 4
    | WillConcede = 5
    | AcceptsDraw = 6
    | Victorious = 7

type PlayerDto = {
    id : int
    gameId : int
    userId : Nullable<int>
    kind : PlayerKindDto

    [<Required>]
    name : string

    status : PlayerStatusDto
    colorId : Nullable<int>
    startingRegion : Nullable<int>
    startingTurnNumber : Nullable<int>
}

type PieceKindDto = 
    | Conduit = 1
    | Thug = 2
    | Scientist = 3
    | Hunter = 4
    | Diplomat = 5
    | Reaper = 6
    | Corpse = 7

type PieceDto = {
    id : int
    kind : PieceKindDto
    playerId : Nullable<int>
    originalPlayerId : int
    cellId : int
}

type SelectionKindDto = 
    | Subject = 1
    | Move = 2
    | Target = 3
    | Drop = 4
    | Vacate = 5

type SelectionDto = {
    kind : SelectionKindDto
    cellId : int
    pieceId : Nullable<int>
}

type TurnStatusDto =
    | AwaitingSelection = 1
    | AwaitingCommit = 2
    | DeadEnd = 3 

// Turn needs to be a class so null can be used for instances
[<AllowNullLiteral>]
type TurnDto(status : TurnStatusDto,
            selections : List<SelectionDto>,
            selectionOptions : List<int>,
            requiredSelectionKind : Nullable<SelectionKindDto>) =
    member __.Status = status

    [<Required>]
    member __.Selections = selections

    [<Required>]
    member __.SelectionOptions = selectionOptions

    member __.RequiredSelectionKind = requiredSelectionKind

type GameStatusDto = 
    | Pending = 1
    | InProgress = 2
    | Canceled = 3
    | Over = 4

[<CLIMutable>]
type GameParametersDto = {
    // Nullable
    description : string
    
    regionCount : int
    
    isPublic : bool
    
    allowGuests : bool
}

type GameDto = {
    id : int

    [<Required>]
    createdBy : CreationSourceDto

    [<Required>]
    parameters : GameParametersDto

    status : GameStatusDto

    [<Required>]
    players : List<PlayerDto>

    // Nullable
    pieces : List<PieceDto>

    // Nullable
    turnCycle : List<int>

    // Nullable
    currentTurn : TurnDto
}