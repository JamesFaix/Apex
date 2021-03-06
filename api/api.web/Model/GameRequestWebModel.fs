﻿namespace Djambi.Api.Web.Model

open System
open System.ComponentModel
open Djambi.Api.Enums
open System.ComponentModel.DataAnnotations

[<CLIMutable>]
type CreatePlayerRequestDto = {
    [<Required>]
    kind : PlayerKind
    
    userId : Nullable<int>

    // Nullable
    [<StringLength(20, MinimumLength = 1)>]
    [<RegularExpression("[a-zA-Z0-9\-_]+")>]
    name : string
}

[<CLIMutable>]
type SelectionRequestDto = {
    [<Required>]
    cellId : int
}

[<CLIMutable>]
type EventsQueryDto = {
    maxResults : Nullable<int>
    direction : ListSortDirection
    thresholdTime : Nullable<DateTime>
    thresholdEventId : Nullable<int>
}