namespace Apex.Api.Logic.Services

open System
open Apex.Api.Common.Control
open Apex.Api.Db.Interfaces
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open System.Threading.Tasks
open FSharp.Control.Tasks
open System.Security.Authentication

type SessionService(encryptionService : IEncryptionService,
                    sessionRepo : ISessionRepository,
                    userRepo : IUserRepository) =

    let maxFailedLoginAttempts = 5
    let accountLockTimeout = TimeSpan.FromHours(1.0)
    let sessionTimeout = TimeSpan.FromHours(1.0)

    interface ISessionService with
        member __.openSession request = 
            let isWithinLockTimeoutPeriod (u : UserDetails) =
                u.lastFailedLoginAttemptOn.IsNone
                || DateTime.UtcNow - u.lastFailedLoginAttemptOn.Value < accountLockTimeout

            let errorIfLocked (user : UserDetails) =
                if user.failedLoginAttempts >= maxFailedLoginAttempts
                    && isWithinLockTimeoutPeriod user
                then raise <| AuthenticationException("Account locked.")
                else ()

            let errorIfInvalidPassword (user : UserDetails) =
                let result = encryptionService.check (user.password, request.password)
                if result.verified
                then Task.FromResult ()
                else
                    let attempts =
                        if isWithinLockTimeoutPeriod user
                        then user.failedLoginAttempts + 1
                        else 1

                    let request = UpdateFailedLoginsRequest.increment (user.id, attempts)
                    task {
                        let! _ = userRepo.updateFailedLoginAttempts request
                        raise <| AuthenticationException("Incorrect password.")
                    }

            let deleteSessionForUser (userId : int) : Task<unit> =            
                task {
                    match! sessionRepo.getSession (SessionQuery.byUserId userId) with
                    | None -> return ()
                    | Some session -> return! sessionRepo.deleteSession session.token
                }

            task {
                match! userRepo.getUserByName request.username with
                | None -> return raise <| AuthenticationException("User does not exist.")
                | Some user ->
                    errorIfLocked user
                    let! _ = errorIfInvalidPassword user

                    //If a session already exists for this user, delete it
                    let! _ = deleteSessionForUser user.id

                    //Create a new session            
                    let request : CreateSessionRequest =
                        {
                            userId = user.id
                            token = Guid.NewGuid().ToString()
                            expiresOn = DateTime.UtcNow.Add(sessionTimeout)
                        }
                    let! session = sessionRepo.createSession request
                    let! _ = userRepo.updateFailedLoginAttempts (UpdateFailedLoginsRequest.reset user.id)

                    return session
            }

        member __.closeSession session = 
            sessionRepo.deleteSession session.token

        member __.getAndRenewSession token =
            task {
                match! sessionRepo.getSession (SessionQuery.byToken token) with
                | None -> return None
                | Some session ->
                    if session.expiresOn <= DateTime.UtcNow
                    then
                        let! _ = sessionRepo.deleteSession session.token
                        return None
                    else
                        let! session = sessionRepo.renewSessionExpiration(session.id, DateTime.UtcNow.Add(sessionTimeout))
                        return Some session          
            }