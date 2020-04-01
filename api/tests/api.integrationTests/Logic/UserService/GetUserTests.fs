namespace Apex.Api.IntegrationTests.Logic.userServ

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Enums
open Apex.Api.Logic.Services

type GetUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get user should work if EditUsers`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = host.Get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue
            let! currentUser = createUser() |> AsyncHttpResult.thenValue

            let session = getSessionForUser (currentUser |> UserDetails.hideDetails) |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! userResponse = host.Get<UserService>().getUser user.id session
                                |> AsyncHttpResult.thenValue

            //Assert
            userResponse.id |> shouldBe user.id
            userResponse.name |> shouldBe user.name
        }

    [<Fact>]
    let ``Get user should fail if not EditUsers`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = host.Get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue
            let! currentUser = createUser() |> AsyncHttpResult.thenValue

            let session = getSessionForUser (currentUser |> UserDetails.hideDetails) |> TestUtilities.setSessionPrivileges []

            //Act
            let! error = host.Get<UserService>().getUser user.id session

            //Assert
            error |> shouldBeError 403 Security.noPrivilegeOrSelfErrorMessage
        }

    [<Fact>]
    let ``Get user should fail is user doesn't exist`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! user = createUser() |> AsyncHttpResult.thenValue
            let session = getSessionForUser (user |> UserDetails.hideDetails) |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! error = host.Get<UserService>().getUser Int32.MinValue session

            //Assert
            error |> shouldBeError 404 "User not found."
        }