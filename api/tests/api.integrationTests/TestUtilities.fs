[<AutoOpen>]
module Djambi.Api.IntegrationTests.TestUtilities

open System
open System.Linq
open FSharp.Control.Tasks
open Microsoft.Extensions.Configuration
open Serilog
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces
open Djambi.Api.Logic
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Model

let private config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("DJAMBI_")
        .Build()

let connectionString = config.["djambiConnectionString"]

let log = LoggerConfiguration().CreateLogger()
let db = DbRoot(connectionString) :> IDbRoot
let services = ServiceRoot(db, log)
let managers = ManagerRoot(db, services) :> IManagerRoot

let random = Random()
let randomAlphanumericString (length : int) : string =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    let charsLen = chars.Length
    let randomChars = 
        [|for i in 0..length-1 -> 
            chars.[random.Next(charsLen)]
        |]
    String(randomChars)

let getCreateUserRequest() : CreateUserRequest =
    {
        name = "Test_" + (randomAlphanumericString 15)
        password = randomAlphanumericString 20
    }

let getLoginRequest(userRequest : CreateUserRequest) : LoginRequest =
    {
        username = userRequest.name
        password = userRequest.password
    }

let getGameParameters() : GameParameters =
    {
        regionCount = 3
        description = Some "Test"
        isPublic = false
        allowGuests = false
    }

let getCreateGameRequest (userId : int) : CreateGameRequest =
    {
        parameters = getGameParameters()
        createdByUserId = userId
    }

let getCreatePlayerRequest : CreatePlayerRequest =
    {
        kind = PlayerKind.User
        userId = None
        name = None
    }

let adminUserId = 1

let getSessionForUser (userId : int) : Session =
    {
        user =
            {
                id = userId
                name = ""
                privileges = []
            }
        id = 0
        token = ""
        createdOn = DateTime.MinValue
        expiresOn = DateTime.MinValue
    }

let createUser() : UserDetails AsyncHttpResult =
    let userRequest = getCreateUserRequest()
    services.users.createUser userRequest None

let createuserSessionAndGame(allowGuests : bool) : (UserDetails * Session * Game) AsyncHttpResult =
    task {
        let! user = createUser() |> thenValue

        let session = getSessionForUser user.id

        let parameters = { getGameParameters() with allowGuests = allowGuests }
        let! game = managers.games.createGame parameters session
                     |> thenValue

        return Ok <| (user, session, game)
    }

let fillEmptyPlayerSlots (game : Game) : Game AsyncHttpResult =
    task {
        let missingPlayerCount = game.parameters.regionCount - game.players.Length
        for i in Enumerable.Range(0, missingPlayerCount) do
            let name = sprintf "neutral%i" (i+1)
            let request = CreatePlayerRequest.neutral name
            let! _ = db.games.addPlayer (game.id, request) |> thenValue
            ()

        return! db.games.getGame game.id
    }

let emptyEventRequest : CreateEventRequest =
    {
        kind = EventKind.CellSelected //Kind shouldn't matter
        effects = []
        createdByUserId = 1
        actingPlayerId = None
    }

let createEventRequest (effects : Effect list) : CreateEventRequest =
    { emptyEventRequest with effects = effects }

let defaultGame : Game =
    {
        id = 0
        status = GameStatus.Pending
        createdBy = {
            userId = 0
            userName = ""
            time = DateTime.MinValue
        }
        parameters =
            {
                allowGuests = false
                description = None
                isPublic = false
                regionCount = 0
            }
        players = []
        pieces = []
        turnCycle = []
        currentTurn = None
    }

let setSessionUserId (userId : int) (session : Session) : Session=
    { session with
        user = { session.user with
                    id = userId
               }
    }

let setSessionPrivileges (privileges : Privilege list) (session : Session) : Session =
    { session with
        user = { session.user with
                    privileges = privileges
        }
    }