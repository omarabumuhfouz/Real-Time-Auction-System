using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Features.Authentication.Commands.Logout;
using System.Security.Claims;

namespace MazadZone.Api.Endpoints.Auth;

public sealed record LogoutRequest(string RefreshToken, bool IsLogoutFromAllDevices);

public static class Logout
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("logout", HandleAsync)
            .RequireAuthorization() 
            .WithSummary("Invalidate user session")
            .WithDescription("Logs out the current authenticated user by revoking their refresh token. Set 'isLogoutFromAllDevices' to true to invalidate all active sessions for this user.")
            .Accepts<LogoutRequest>("application/json") 
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

    }

    private static async Task<IResult> HandleAsync(
        [FromBody] LogoutRequest request,
        [FromServices] ISender sender,
        BoundUserId boundUserId,
        CancellationToken ct)
    {

        var command = new LogoutCommand(
            boundUserId.Value,
            request.RefreshToken,
            request.IsLogoutFromAllDevices);

        var result = await sender.Send(command, ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem()
        );
    }
}