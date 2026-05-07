using MazadZone.Api.Contracts.Auth;
using MazadZone.Application.Features.Authentication.Commands.Login;
using MazadZone.Application.Features.Authentication.Commands.Logout;
using MazadZone.Application.Features.Authentication.Commands.RefreshToken;
using MazadZone.Application.Features.Authentication.DTOs;
using MazadZone.Domain.Users.ValueObjects;
using MechanicShop.Api.Extensions;
using System.Security.Claims;

namespace MazadZone.Api.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var authGroup = app.MapGroup("api/v{version:apiVersion}/auth")
                           .WithApiVersionSet(versionSet)
                           .MapToApiVersion(1, 0)
                           .WithTags("Authentication");

        // 1. Login
        authGroup.MapPost("login", LoginAsync)
                 .Produces<TokenDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);

        // 2. Refresh Token
        authGroup.MapPost("refresh", RefreshAsync)
                 .Produces<TokenDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);

        // 3. Logout
        authGroup.MapPost("logout", LogoutAsync)
                 .RequireAuthorization()
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password), ct);

        return result.Match(
            onValue: value => Results.Ok(value),
            onError: errors => errors.ToProblem()
        );
    }

    private static async Task<IResult> RefreshAsync(
        [FromBody] RefreshTokenRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new RefreshTokenCommand(request.RefreshToken), ct);

        // We return 200 OK with the new Access + Refresh token pair
        return result.Match(
            onValue: value => Results.Ok(value),
            onError: errors => errors.ToProblem()
        );
    }

    private static async Task<IResult> LogoutAsync(
        [FromBody] LogoutRequest request,
        [FromServices] ISender sender,
        ClaimsPrincipal userPrincipal,
        CancellationToken ct)
    {
        var userId = userPrincipal.GetUserId();

        var command = new LogoutCommand(
            UserId.Load(userId), 
            request.RefreshToken, 
            request.IsLogoutFromAllDevices);

        var result = await sender.Send(command, ct);
        return result.Match(
             onValue: _ => Results.NoContent(),
             onError: errors => errors.ToProblem()
        );
    }
}

// Data Transfer Objects for the API Request Body
