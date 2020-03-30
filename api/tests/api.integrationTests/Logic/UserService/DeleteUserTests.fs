namespace Apex.Api.IntegrationTests.Logic.userServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Enums
open Apex.Api.Logic.Services

type DeleteUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Delete user should work if deleting self`` =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = Host.get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser user.id |> TestUtilities.setSessionPrivileges []

            //Act
            let! response = Host.get<UserService>().deleteUser user.id session

            //Assert
            response |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete user should work if EditUsers and deleting other user`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = Host.get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser (user.id + 1) |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! response = Host.get<UserService>().deleteUser user.id session

            //Assert
            response |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete user should fail if not EditUsers and deleting other user`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = Host.get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser (user.id + 1) |> TestUtilities.setSessionPrivileges []

            //Act
            let! response = Host.get<UserService>().deleteUser user.id session

            //Assert
            response |> shouldBeError 403 Security.noPrivilegeOrSelfErrorMessage
        }

    [<Fact>]
    let ``Delete user should fail if already deleted`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = Host.get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            let! _ = Host.get<UserService>().deleteUser user.id session

            //Act
            let! response = Host.get<UserService>().deleteUser user.id session

            //Assert
            response |> shouldBeError 404 "User not found."
        }