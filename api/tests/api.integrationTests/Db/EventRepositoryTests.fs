﻿namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Logic.Managers

type EventRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should add players``() =
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let player : Player = 
                {
                    id = 0
                    gameId = game.id
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = "p2"
                    status = PlayerStatus.Pending
                    colorId = None
                    startingRegion = None
                    startingTurnNumber = None
                }

            let newGame = { game with players = [player] }

            //Act
            let! resp = EventRepository.persistEvent (TestUtilities.emptyEventRequest, game, newGame) |> thenValue

            //Assert
            let persistedGame = resp.game

            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 1

            let persistedPlayer = persistedGame.players.Head
            persistedPlayer.id |> shouldNotBe 0
            { persistedPlayer with id = player.id } |> shouldBe player
        }

    [<Fact>]
    let ``Should remove players``() =
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let playerRequest = 
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = GameRepository.addPlayer(game.id, playerRequest) |> thenValue
            let! game = GameRepository.getGame game.id |> thenValue

            let newGame = { game with players = [] }

            //Act
            let! resp = EventRepository.persistEvent (TestUtilities.emptyEventRequest, game, newGame) |> thenValue

            //Assert
            let persistedGame = resp.game

            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 0
        }

    [<Fact>]
    let ``Should update players``() =
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let p2Request = 
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = GameRepository.addPlayer(game.id, p2Request) |> thenValue
            let! game = GameRepository.getGame game.id |> thenValue

            let oldP2 = game.players.[1]
            oldP2.status |> shouldBe PlayerStatus.Pending
            oldP2.startingRegion |> shouldBe None
            oldP2.startingTurnNumber |> shouldBe None
            oldP2.colorId |> shouldBe None

            let newP2 = 
                { oldP2 with
                    status = PlayerStatus.Alive
                    startingRegion = Some 1
                    startingTurnNumber = Some 1
                    colorId = Some 1
                }

            let newGame = 
                { game with 
                    players = [
                        game.players.[0]
                        newP2
                    ]
                }

            //Act
            let! resp = EventRepository.persistEvent (TestUtilities.emptyEventRequest, game, newGame) |> thenValue

            //Assert
            let persistedGame = resp.game

            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 2
            persistedGame.players.[0] |> shouldBe game.players.[0]
            persistedGame.players.[1] |> shouldBe newP2
        }

    [<Fact>]
    let ``Should update game``() =
        task {
            //Arrange
            let! (_, _, game) = TestUtilities.createuserSessionAndGame(false) |> thenValue

            game.status |> shouldBe GameStatus.Pending
            game.parameters |> shouldBe 
                {
                    allowGuests = false
                    isPublic = false
                    regionCount = 3
                    description = Some "Test"
                }
            game.currentTurn |> shouldBe None
            game.turnCycle |> shouldBe []
            game.pieces |> shouldBe []
            
            let newGame = 
                { game with
                    status = GameStatus.Aborted
                    parameters =    
                        {
                            allowGuests = true
                            isPublic = true
                            regionCount = 8
                            description = None
                        }
                    currentTurn = Some Turn.empty
                    turnCycle = [ 1 ]
                    pieces = [ 
                        {
                            id = 0
                            kind = PieceKind.Assassin
                            cellId = 0
                            playerId = None
                            originalPlayerId = 0
                        }
                    ]
                }

            //Act
            let! resp = EventRepository.persistEvent (TestUtilities.emptyEventRequest, game, newGame) |> thenValue

            //Assert
            let persistedGame = resp.game
            persistedGame |> shouldBe newGame
        }

    [<Fact>]
    let ``Should rollback all effects if any errors``() =
        task {
            //Arrange
            let! (_, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            //Attempt to add 2 players with the same name
            //This will violate a unique index in SQL and fail the second player

            let playerRequest = 
                {
                    userId = None
                    name = Some "test"
                    kind = PlayerKind.Neutral
                }

            let effects = 
                [
                    PlayerAddedEffect.fromRequest playerRequest
                    PlayerAddedEffect.fromRequest playerRequest
                ]
            let event = TestUtilities.createEventRequest(effects) //EventKind doesn't matter

            let newGame = EventService.applyEvent game event
            
            //Act
            let! result = EventRepository.persistEvent (TestUtilities.emptyEventRequest, game, newGame)

            //Assert
            result |> shouldBeError 409 "Conflict when attempting to write Event."

            let! persistedGame = GameRepository.getGame game.id |> thenValue
            persistedGame.players.Length |> shouldBe 1 //Just the creator
            persistedGame.players |> shouldNotExist (fun p -> p.name = playerRequest.name.Value)
        }

    [<Fact>]
    let ``Should get events``() =
        task {
            //Arrange
            let! (user, session, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let p2Request = 
                {
                    userId = Some user.id
                    name = Some "p2"
                    kind = PlayerKind.Guest
                }

            let p3Request = { p2Request with name = Some "p3" }

            let! _ = GameManager.addPlayer game.id p2Request session |> thenValue
            let! _ = GameManager.addPlayer game.id p3Request session |> thenValue
            
            let query : EventsQuery = 
                {
                    maxResults = None
                    direction = Ascending
                    thresholdEventId = None
                    thresholdTime = None
                }

            //Act
            let! events = EventRepository.getEvents (game.id, query) |> thenValue

            //Assert
            events.Length |> shouldBe 2

            let e1 = events.[0]
            e1.kind |> shouldBe EventKind.PlayerJoined
            e1.createdByUserId |> shouldBe user.id
            e1.effects.Length |> shouldBe 1
            
            e1.effects.[0] |> shouldBe (PlayerAddedEffect.fromRequest p2Request)
            
            let e2 = events.[1]
            e2.kind |> shouldBe EventKind.PlayerJoined
            e2.createdByUserId |> shouldBe user.id
            e2.effects.Length |> shouldBe 1

            e2.effects.[0] |> shouldBe (PlayerAddedEffect.fromRequest p3Request)
        }