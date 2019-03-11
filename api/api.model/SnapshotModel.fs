﻿[<AutoOpen>]
module Djambi.Api.Model.SnapshotModel

open System
open Djambi.ClientGenerator.Annotations

type Snapshot = 
    {
        id : int
        createdByUserId : int
        createdOn : DateTime
        description : string
        game : Game
        history : Event list
    }
    
[<ClientType(ClientSection.Snapshots)>]
type SnapshotInfo =
    {
        id : int
        createdByUserId : int
        createdOn : DateTime
        description : string    
    }
    
[<ClientType(ClientSection.Snapshots)>]
type CreateSnapshotRequest =
    {
        description : string
    }

type InternalCreateSnapshotRequest = 
    {
        game : Game
        history : Event list
        createdByUserId : int
        description : string
    }

module Snapshot =
    let hideDetails (snapshot : Snapshot) : SnapshotInfo =
        {
            id = snapshot.id
            createdByUserId = snapshot.createdByUserId
            createdOn = snapshot.createdOn
            description = snapshot.description
        }