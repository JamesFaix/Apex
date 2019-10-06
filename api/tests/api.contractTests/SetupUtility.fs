module Djambi.Api.ContractTests.SetupUtility

open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.Extensions.Configuration
open Djambi.Api.WebClient
open Djambi.Api.Model

let config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("DJAMBI_")
        .Build()

let createUserAndSignIn () : (User * string) Task =
    task {
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserClient.createUser createUserRequest |> AsyncResponse.bodyValue
        let request = RequestFactory.loginRequest createUserRequest
        let! sessionResponse = SessionClient.createSession request
        let token = sessionResponse.getToken().Value
        return (user, token)
    }

let loginAsAdmin () : string Task =
    task {
        let request : LoginRequest =
            {
                username = config.["adminUsername"]
                password = config.["adminPassword"]
            }
        let! sessionResponse = SessionClient.createSession request
        return sessionResponse.getToken().Value
    }