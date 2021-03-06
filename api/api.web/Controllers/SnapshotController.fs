﻿namespace Djambi.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Web
open Djambi.Api.Web.Mappings
open Djambi.Api.Web.Model

[<ApiController>]
[<Route("api/games/{gameId}/snapshots")>]
type SnapshotController(manager : ISnapshotManager,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<SnapshotInfoDto>)>]
    member __.CreateSnapshot(gameId : int, [<FromBody>] request : CreateSnapshotRequestDto) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let request = request |> toCreateSnapshotRequest
            let! snapshot = manager.createSnapshot gameId request session
            let dto = snapshot |> toSnapshotInfoDto
            return OkObjectResult(dto) :> IActionResult
        }
        
    [<HttpGet>]
    [<ProducesResponseType(200, Type = typeof<SnapshotInfoDto[]>)>]
    member __.GetSnapshots(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! snapshots = manager.getSnapshotsForGame gameId session
            let dtos = snapshots |> List.map toSnapshotInfoDto
            return OkObjectResult(dtos) :> IActionResult
        }
    
    [<HttpDelete("{snapshotId}")>]
    [<ProducesResponseType(204)>]
    member __.DeleteSnapshot(gameId : int, snapshotId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! _ = manager.deleteSnapshot gameId snapshotId session
            return NoContentResult() :> IActionResult
        }
    
    [<HttpPost("{snapshotId}/load")>]
    [<ProducesResponseType(204)>]
    member __.LoadSnapshot(gameId : int, snapshotId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! _ = manager.loadSnapshot gameId snapshotId session
            return NoContentResult() :> IActionResult
        }