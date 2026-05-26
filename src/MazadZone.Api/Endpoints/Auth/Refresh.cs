using Grpc.Core;
using MazadZone.Application.Features.Authentication.Commands.RefreshToken;
using MazadZone.Application.Features.Authentication.DTOs;

namespace MazadZone.Api.Endpoints.Auth;

public sealed record RefreshTokenRequest(string RefreshToken);

public static class Refresh
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("refresh", HandleAsync)
            .AllowAnonymous() 
            .WithSummary("Refresh an expired access token")
            .WithDescription("Exchanges a valid refresh token for a new set of access and refresh tokens. Use this endpoint when the short-lived JWT access token expires.")
            .Accepts<RefreshTokenRequest>("application/json")
            .Produces<TokenDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RefreshTokenRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new RefreshTokenCommand(request.RefreshToken), ct);

        return result.Match(
            onValue: value => Results.Ok(value),
            onError: errors => errors.ToProblem()
        );
    }
}