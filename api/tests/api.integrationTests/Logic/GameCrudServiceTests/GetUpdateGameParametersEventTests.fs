namespace Apex.Api.IntegrationTests.Logic.GameCrudService

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open Apex.Api.IntegrationTests
open Apex.Api.Logic
open Apex.Api.Logic.Services
open Apex.Api.Model

type GetUpdateGameParametersEventTests() =
    inherit TestsBase()

    let host = HostFactory.createHost()
    let makePlayer gameId player = host.Get<IPlayerRepository>().addPlayer(gameId, player, true)

    [<Fact>]
    let ``Should work``() =
        task {
            //Arrange
            let! (_, session, game) = TestUtilities.createuserSessionAndGame(true)

            let newParameters =
                {
                    allowGuests = false
                    isPublic = true
                    description = Some "new description"
                    regionCount = 5
                }

            //Act
            let event = host.Get<GameCrudService>().getUpdateGameParametersEvent (game, newParameters) session

            //Assert
            event.kind |> shouldBe EventKind.GameParametersChanged
            event.effects.Length |> shouldBe 1

            match event.effects.[0] with
            | Effect.ParametersChanged f ->
                f.oldValue |> shouldBe game.parameters
                f.newValue |> shouldBe newParameters

            | _ -> failwith "Incorrect effects"
        }


    [<Fact>]
    let ``Should have effect for truncating extra players if region count being lowered``() =
        task {
            //Arrange
            let! (user, session, game) = TestUtilities.createuserSessionAndGame(true)

            let newGame =
                { game with parameters =
                            { game.parameters with regionCount = 4 }
                }

            let! _ = host.Get<IGameRepository>().updateGame newGame

            for n in [2..4] do
                let player =
                    { TestUtilities.getCreatePlayerRequest with
                        userId = Some user.id;
                        kind = PlayerKind.Guest;
                        name = Some (sprintf "p%i" n)
                    } |> CreatePlayerRequest.toPlayer None
                let! _ = makePlayer game.id player
                ()

            let! game = host.Get<IGameRepository>().getGame game.id

            let newParameters = { game.parameters with regionCount = 3}

            //Act
            let event = host.Get<GameCrudService>().getUpdateGameParametersEvent (game, newParameters) session

            //Assert
            event.kind |> shouldBe EventKind.GameParametersChanged
            event.effects.Length |> shouldBe 2

            match (event.effects.[0], event.effects.[1]) with
            | (Effect.ParametersChanged f1, Effect.PlayerRemoved f2) ->
                f1.oldValue |> shouldBe game.parameters
                f1.newValue |> shouldBe newParameters
                f2.oldPlayer.id |> shouldBe game.players.[3].id

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should have effect for removing guests if allowGuests is being disabled``() =
        task {
            //Arrange
            let! (user, session, game) = TestUtilities.createuserSessionAndGame(true)

            for n in [2..3] do
                let player =
                    { TestUtilities.getCreatePlayerRequest with
                        userId = Some user.id;
                        kind = PlayerKind.Guest;
                        name = Some (sprintf "p%i" n)
                    } |> CreatePlayerRequest.toPlayer None
                let! _ = makePlayer game.id player
                ()

            let! game = host.Get<IGameRepository>().getGame game.id

            let newParameters = { game.parameters with allowGuests = false }

            //Act
            let event = host.Get<GameCrudService>().getUpdateGameParametersEvent (game, newParameters) session

            //Assert
            event.kind |> shouldBe EventKind.GameParametersChanged
            event.effects.Length |> shouldBe 3

            match (event.effects.[0], event.effects.[1], event.effects.[2]) with
            | (Effect.ParametersChanged f1, Effect.PlayerRemoved f2, Effect.PlayerRemoved f3) ->
                f1.oldValue |> shouldBe game.parameters
                f1.newValue |> shouldBe newParameters
                f2.oldPlayer.id |> shouldBe game.players.[1].id
                f3.oldPlayer.id |> shouldBe game.players.[2].id

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should fail if game not pending``() =
        task {
            //Arrange
            let! (_, session, game) = TestUtilities.createuserSessionAndGame(true)
            let newGame = { game with status = GameStatus.InProgress }
            let! _ = host.Get<IGameRepository>().updateGame newGame
            let! game = host.Get<IGameRepository>().getGame game.id
            let newParameters = game.parameters

            //Act/Assert
            let ex = Assert.Throws<HttpException>(fun () -> 
                host.Get<GameCrudService>().getUpdateGameParametersEvent (game, newParameters) session 
                |> ignore
            )

            ex.statusCode |> shouldBe 400
            ex.Message |> shouldBe "Cannot change game parameters unless game is Pending."
        }

    [<Fact>]
    let ``Should fail if not game creator and no EditPendingGames privilege``() =
        task {
            //Arrange
            let! (_, session, game) = TestUtilities.createuserSessionAndGame(true)

            let newParameters = game.parameters

            let otherSession = session |> TestUtilities.setSessionUserId (session.user.id + 1)

            //Act/Assert
            let ex = Assert.Throws<UnauthorizedAccessException>(fun () -> 
                host.Get<GameCrudService>().getUpdateGameParametersEvent (game, newParameters) otherSession
                |> ignore
            )

            ex.Message |> shouldBe Security.noPrivilegeOrCreatorErrorMessage
        }