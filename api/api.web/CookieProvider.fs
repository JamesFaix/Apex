namespace Apex.Api.Web

open System
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Options
open Apex.Api.Common
open Apex.Api.Model.Configuration

type CookieProvider(options : IOptions<ApiSettings>) =
    let cookieName = options.Value.cookieName

    member __.AppendCookie (ctx : HttpContext) (token : string, expiration : DateTime) =
        let cookieOptions = CookieOptions()
        cookieOptions.Domain <- options.Value.cookieDomain
        cookieOptions.Path <- "/"
        cookieOptions.Secure <- false
        cookieOptions.HttpOnly <- true
        cookieOptions.Expires <- DateTimeOffset(expiration) |> Nullable.ofValue
        ctx.Response.Cookies.Append(cookieName, token, cookieOptions);

    member x.AppendEmptyCookie (ctx : HttpContext) =
        x.AppendCookie ctx ("", DateTime.MinValue)
