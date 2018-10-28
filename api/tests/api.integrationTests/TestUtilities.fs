﻿[<AutoOpen>]
module Djambi.Api.IntegrationTests.TestUtilities

open System
open Microsoft.Extensions.Configuration
open Djambi.Api.Model.UserModel
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.SessionModel
open Djambi.Utilities

let private config =
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .AddJsonFile(Environment.environmentConfigPath(6), false)
        .Build()

let connectionString =
    config.GetConnectionString("Main")
            .Replace("{sqlAddress}", config.["sqlAddress"])

let getCreateUserRequest() : CreateUserRequest =
    {
        name = "Test_" + Guid.NewGuid().ToString()
        password = Guid.NewGuid().ToString()
    }

let getLoginRequest(userRequest : CreateUserRequest) : LoginRequest =
    {
        username = userRequest.name
        password = userRequest.password
    }

let getCreateLobbyRequest() : CreateLobbyRequest =
    {
        regionCount = 3
        description = Some "Test"
        createdByUserId = 1
        isPublic = false
        allowGuests = false
    }

let adminUserId = 1

let getSessionForUser (userId : int) : Session =
    {
        userId = userId
        isAdmin = false
        id = 0
        token = ""
        createdOn = DateTime.MinValue
        expiresOn = DateTime.MinValue
    }