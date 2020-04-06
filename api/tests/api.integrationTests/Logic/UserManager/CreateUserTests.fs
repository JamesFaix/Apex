namespace Apex.Api.IntegrationTests.Logic.UserManager

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Enums
open Apex.Api.Logic.Interfaces
open System.Threading.Tasks

type CreateUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create user should work if not signed in``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()

            //Act
            let! user = host.Get<IUserManager>().createUser request None

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }

    [<Fact>]
    let ``Create user should fail if name conflict``() =
        let host = HostFactory.createHost()
        let request = getCreateUserRequest()
        
        task {
            //Arrange
            let! _ = host.Get<IUserManager>().createUser request None

            //Act/Assert
            let! ex = Assert.ThrowsAsync<HttpException>(fun () -> 
                task {
                    let! _ = host.Get<IUserManager>().createUser request None            
                    return ()
                } :> Task
            )
            
            ex.statusCode |> shouldBe 409
            ex.Message |> shouldBe "Conflict when attempting to write User."
        }

    [<Fact>]
    let ``Create user should fail if signed in and not EditUsers``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser()
            let request = getCreateUserRequest()
            let session = getSessionForUser currentUser |> TestUtilities.setSessionPrivileges []

            //Act/Assert

            let! ex = Assert.ThrowsAsync<HttpException>(fun () -> 
                task {
                    let! _ = host.Get<IUserManager>().createUser request (Some session)
                    return ()
                } :> Task
            )

            ex.statusCode |> shouldBe 403
            ex.Message |> shouldBe "Cannot create user if logged in."
        }

    [<Fact>]
    let ``Create user should work if signed in and EditUsers``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser()
            let request = getCreateUserRequest()
            let session = getSessionForUser currentUser |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! user = host.Get<IUserManager>().createUser request (Some session)

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }