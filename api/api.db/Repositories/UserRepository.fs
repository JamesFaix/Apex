﻿namespace Apex.Api.Db.Repositories

open Apex.Api.Db.Interfaces
open System
open Apex.Api.Db.Model
open FSharp.Control.Tasks
open Apex.Api.Db.Mappings
open Microsoft.EntityFrameworkCore
open System.Data

type UserRepository(context : ApexDbContext) =    
    let nameConflictMessage = 
        "The instance of entity type 'UserSqlModel' cannot be tracked because " + 
        "another instance with the same key value for {'Name'} is already being tracked."

    interface IUserRepository with
        member __.getUser userId =
            task {
                match! context.Users.FindAsync(userId) with
                | null -> return None
                | _ as user -> return user |> toUserDetails |> Some
            }

        member __.getUserByName name =
            task {
                match! context.Users.SingleOrDefaultAsync(fun x -> name.ToLower() = x.Name.ToLower()) with
                | null -> return None
                | _ as user -> return user |> toUserDetails |> Some
            }

        member __.createUser request =
            task {
                let u = UserSqlModel()
                u.Name <- request.name
                u.Password <- request.password
                u.FailedLoginAttempts <- 0uy
                u.LastFailedLoginAttemptOn <- Nullable<DateTime>()
                u.CreatedOn <- DateTime.UtcNow

                try 
                    let! _ = context.Users.AddAsync(u)
                    let! _ = context.SaveChangesAsync()
                    return u |> toUserDetails
                with
                | :? InvalidOperationException as ex when ex.Message.StartsWith(nameConflictMessage) ->
                    return raise <| DuplicateNameException("User name taken.")
            }

        member __.deleteUser userId = 
            task {
                match! context.Users.FindAsync(userId) with
                | null -> return ()
                | _ as u ->
                    context.Users.Remove(u) |> ignore
                    let! _ = context.SaveChangesAsync()
                    return ()
            }

        member __.updateFailedLoginAttempts request =
            task {
                let! u = context.Users.FindAsync(request.userId)
                u.FailedLoginAttempts <- byte request.failedLoginAttempts
                u.LastFailedLoginAttemptOn <- request.lastFailedLoginAttemptOn |> Option.toNullable
                context.Users.Update(u) |> ignore
                let! _ = context.SaveChangesAsync()
                return ()
            }