﻿namespace Djambi.Api.IntegrationTests.Logic.UserService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type DeleteUserTests() =
    inherit TestsBase()

    [<Theory>]
    [<InlineData(true)>]
    [<InlineData(false)>]
    let ``Delete user should work if deleting self`` (isAdmin : bool) =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = UserService.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser user.id |> TestUtilities.setSessionIsAdmin isAdmin

            //Act
            let! response = UserService.deleteUser user.id session

            //Assert
            response |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete user should work if admin and deleting other user`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = UserService.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser (user.id + 1) |> TestUtilities.setSessionIsAdmin true

            //Act
            let! response = UserService.deleteUser user.id session

            //Assert
            response |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete user should fail if not admin and deleting other user`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = UserService.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser (user.id + 1) |> TestUtilities.setSessionIsAdmin false

            //Act
            let! response = UserService.deleteUser user.id session

            //Assert
            response |> shouldBeError 403 SecurityService.notAdminOrSelfErrorMessage
        }

    [<Fact>]
    let ``Delete user should fail if already deleted`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = UserService.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser 1 |> TestUtilities.setSessionIsAdmin true

            let! _ = UserService.deleteUser user.id session

            //Act
            let! response = UserService.deleteUser user.id session

            //Assert
            response |> shouldBeError 404 "User not found."
        }