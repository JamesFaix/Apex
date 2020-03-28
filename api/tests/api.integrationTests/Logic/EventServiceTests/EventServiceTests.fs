namespace Apex.Api.IntegrationTests.Logic.eventServTests

open Xunit
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Enums

//TODO: Move to unit test project

type EventServiceTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should apply effects in order``() =
        //Arrange
        let game = TestUtilities.defaultGame

        let effects =
            [
                //If the status InProgress is used here additional setup is required to make `game` valid
                Effect.GameStatusChanged { oldValue = GameStatus.Pending; newValue = GameStatus.Canceled }
                Effect.GameStatusChanged { oldValue = GameStatus.Canceled; newValue = GameStatus.Over }
            ]
        let eventRequest = TestUtilities.createEventRequest(effects) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        newGame.status |> shouldBe GameStatus.Over

    [<Fact>]
    let ``Should apply CurrentTurnChanged effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newTurn = Some Turn.empty
        let effect = Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = newTurn }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.currentTurn |> shouldBe None

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with currentTurn = newGame.currentTurn } |> shouldBe newGame

        newGame.currentTurn |> shouldBe newTurn

    [<Fact>]
    let ``Should apply GameStatusChanged effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newStatus = GameStatus.Canceled //Can't use InProgress here because that case is more complicated
        let effect = Effect.GameStatusChanged { oldValue = game.status; newValue = newStatus }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.status |> shouldBe GameStatus.Pending

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with status = newGame.status } |> shouldBe newGame

        newGame.status |> shouldBe newStatus

    [<Fact>]
    let ``Should apply NeutralPlayerAdded effect``() =
        //Arrange
        let game = { TestUtilities.defaultGame with id = 5 }
        let effect = Effect.NeutralPlayerAdded { name = "p2"; placeholderPlayerId = -1 }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.players.Length |> shouldBe 0

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with players = newGame.players } |> shouldBe newGame

        newGame.players.Length |> shouldBe 1
        let p = newGame.players.Head
        p.id |> shouldBe -1 //The real ID is generated when the event is persisted
        p.gameId |> shouldBe newGame.id
        p.userId |> shouldBe None
        p.kind |> shouldBe PlayerKind.Neutral
        p.name |> shouldBe "p2"

        //These are assigned at game start
        p.status |> shouldBe PlayerStatus.Pending
        p.colorId |> shouldBe None
        p.startingRegion |> shouldBe None
        p.startingTurnNumber |> shouldBe None

    [<Fact>]
    let ``Should apply ParametersChanged effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newParameters =
            {
                allowGuests = true
                isPublic = true
                description = Some "test"
                regionCount = 8
            }
        let effect = Effect.ParametersChanged { oldValue = game.parameters; newValue = newParameters }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.parameters |> shouldBe
            {
                allowGuests = false
                isPublic = false
                description = None
                regionCount = 0
            }

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with parameters = newGame.parameters } |> shouldBe newGame

        newGame.parameters |> shouldBe newParameters

    [<Fact>]
    let ``Should apply PieceAbandoned effect``() =
        //Arrange
        let pieces : Piece list =
            [
                {
                    id = 1
                    kind = PieceKind.Hunter
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
                {
                    id = 2
                    kind = PieceKind.Hunter
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
            ]
        let game = { TestUtilities.defaultGame with pieces = pieces}
        let effect = Effect.PieceAbandoned { oldPiece = pieces.[0] }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 2

        newGame.pieces.[0] |> shouldBe { pieces.[0] with playerId = None }
        newGame.pieces.[1] |> shouldBe pieces.[1]

    [<Fact>]
    let ``Should apply PieceDropped effect``() =
        //Arrange
        let piece : Piece =
            {
                id = 1
                kind = PieceKind.Hunter
                playerId = Some 0
                originalPlayerId = 0
                cellId = 0
            }
        let game = { TestUtilities.defaultGame with pieces = [piece]}
        let newPiece = { piece with cellId = 3 }
        let effect = Effect.PieceDropped { oldPiece = piece; newPiece = newPiece }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 1

        let newPiece = newGame.pieces.Head
        newPiece |> shouldBe { piece with cellId = newPiece.cellId }

    [<Fact>]
    let ``Should apply PieceEnlisted effect``() =
        //Arrange
        let pieces : Piece list =
            [
                {
                    id = 1
                    kind = PieceKind.Hunter
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
                {
                    id = 2
                    kind = PieceKind.Hunter
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
            ]
        let game = { TestUtilities.defaultGame with pieces = pieces}
        let newPlayerId = 1
        let effect = Effect.PieceEnlisted { oldPiece = pieces.[0]; newPlayerId = newPlayerId }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 2

        newGame.pieces.[0] |> shouldBe { pieces.[0] with playerId = Some newPlayerId }
        newGame.pieces.[1] |> shouldBe pieces.[1]

    [<Fact>]
    let ``Should apply PieceKilled effect``() =
        //Arrange
        let piece : Piece =
            {
                id = 1
                kind = PieceKind.Hunter
                playerId = Some 0
                originalPlayerId = 0
                cellId = 0
            }
        let game = { TestUtilities.defaultGame with pieces = [piece]}

        let effect = Effect.PieceKilled {
            oldPiece = piece
        }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 1

        let newPiece = newGame.pieces.Head
        newPiece |> shouldBe { piece with kind = PieceKind.Corpse; playerId = None }

    [<Fact>]
    let ``Should apply PieceMoved effect``() =
        //Arrange
        let piece : Piece =
            {
                id = 1
                kind = PieceKind.Hunter
                playerId = Some 0
                originalPlayerId = 0
                cellId = 0
            }
        let game = { TestUtilities.defaultGame with pieces = [piece]}
        let newCellId = 3
        let effect = Effect.PieceMoved { oldPiece = piece; newCellId = newCellId }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 1

        let newPiece = newGame.pieces.Head
        newPiece |> shouldBe { piece with cellId = newCellId }

    [<Fact>]
    let ``Should apply PIeceVacated effect``() =
        //Arrange
        let piece : Piece =
            {
                id = 1
                kind = PieceKind.Hunter
                playerId = Some 0
                originalPlayerId = 0
                cellId = 0
            }
        let game = { TestUtilities.defaultGame with pieces = [piece]}
        let newCellId = 3
        let effect = Effect.PieceVacated { oldPiece = piece; newCellId = newCellId }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 1

        let newPiece = newGame.pieces.Head
        newPiece |> shouldBe { piece with cellId = newCellId }

    [<Fact>]
    let ``Should apply PlayerAdded effect``() =
        //Arrange
        let game = { TestUtilities.defaultGame with id = 5 }
        let userId = 1
        let playerRequest = CreatePlayerRequest.user(userId)
        let effect = PlayerAddedEffect.fromRequest playerRequest
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.players.Length |> shouldBe 0

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with players = newGame.players } |> shouldBe newGame

        newGame.players.Length |> shouldBe 1
        let p = newGame.players.Head
        p.id |> shouldBe 0 //This is generated when the event is persisted
        p.gameId |> shouldBe newGame.id
        p.userId |> shouldBe playerRequest.userId
        p.kind |> shouldBe playerRequest.kind
        p.name |> shouldBe "" //This is pulled from the db for User players

        //These are assigned at game start
        p.status |> shouldBe PlayerStatus.Pending
        p.colorId |> shouldBe None
        p.startingRegion |> shouldBe None
        p.startingTurnNumber |> shouldBe None

    [<Fact>]
    let ``Should apply PlayerStatusChanged effect``() =
        //Arrange
        let player : Player =
            {
                id = 1
                kind = PlayerKind.User
                name = "test"
                gameId = 0
                userId = None
                status = PlayerStatus.Pending
                colorId = None
                startingRegion = None
                startingTurnNumber = None
            }
        let game = { TestUtilities.defaultGame with players = [player] }
        let effect = Effect.PlayerStatusChanged {
                playerId = player.id
                oldStatus = player.status
                newStatus = PlayerStatus.Eliminated
            }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with players = newGame.players } |> shouldBe newGame

        newGame.players.Length |> shouldBe 1

        let newPlayer = newGame.players.Head
        newPlayer |> shouldBe { player with status = PlayerStatus.Eliminated }

    [<Fact>]
    let ``Should apply PlayersRemoved effect``() =
        //Arrange
        let players : Player list =
            [
                {
                    id = 1
                    kind = PlayerKind.User
                    name = "p1"
                    gameId = 0
                    userId = None
                    status = PlayerStatus.Pending
                    colorId = None
                    startingRegion = None
                    startingTurnNumber = None
                }
                {
                    id = 2
                    kind = PlayerKind.User
                    name = "p2"
                    gameId = 0
                    userId = None
                    status = PlayerStatus.Pending
                    colorId = None
                    startingRegion = None
                    startingTurnNumber = None
                }
            ]
        let game = { TestUtilities.defaultGame with players = players}
        let effect = Effect.PlayerRemoved { oldPlayer = players.[0] }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with players = newGame.players } |> shouldBe newGame

        newGame.players.Length |> shouldBe 1
        newGame.players.Head |> shouldBe players.[1]

    [<Fact>]
    let ``Should apply TurnCycleAdvanced effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newCycle = [1;2;3]
        let effect = Effect.TurnCycleAdvanced { oldValue = game.turnCycle; newValue = newCycle }
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.turnCycle |> shouldBe []

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

        newGame.turnCycle |> shouldBe newCycle

    [<Fact>]
    let ``Should apply TurnCyclePlayerFellFromPower effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newCycle = [1;2;3]
        let effect = Effect.TurnCyclePlayerFellFromPower { oldValue = game.turnCycle; newValue = newCycle; playerId = 1 } //PlayerID is just informative, doesn't effect processing
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.turnCycle |> shouldBe []

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

        newGame.turnCycle |> shouldBe newCycle

    [<Fact>]
    let ``Should apply TurnCyclePlayerRemoved effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newCycle = [1;2;3]
        let effect = Effect.TurnCyclePlayerRemoved { oldValue = game.turnCycle; newValue = newCycle; playerId = 1 } //PlayerID is just informative, doesn't effect processing
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.turnCycle |> shouldBe []

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

        newGame.turnCycle |> shouldBe newCycle

    [<Fact>]
    let ``Should apply TurnCyclePlayerRoseToPower effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newCycle = [1;2;3]
        let effect = Effect.TurnCyclePlayerRoseToPower { oldValue = game.turnCycle; newValue = newCycle; playerId = 1 } //PlayerID is just informative, doesn't effect processing
        let eventRequest = TestUtilities.createEventRequest([effect]) //Kind doesn't matter

        game.turnCycle |> shouldBe []

        //Act
        let newGame = eventServ.applyEvent game eventRequest

        //Assert
        { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

        newGame.turnCycle |> shouldBe newCycle   