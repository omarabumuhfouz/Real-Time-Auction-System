using MazadZone.Application.Features.Users.Commands.Ban;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Api.Endpoints.Users;

public record BanUserRequest(string Reason);

public static class Ban
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/ban", HandleAsync)
        //    .RequireAuthorization("AdminOnly")
           .WithSummary("Ban a user account")
           .WithDescription("Suspends a user's account, immediately preventing them from logging in, bidding, or managing auctions. A mandatory reason must be provided in the request body. Returns a 409 Conflict if the user is already banned or if domain rules prevent the ban (e.g., trying to ban another super-admin).")
           .Accepts<BanUserRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For malformed GUIDs or missing 'Reason'
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user lacks the AdminOnly policy
           .ProducesProblem(StatusCodes.Status404NotFound) // User does not exist
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UserId id,
        [FromBody] BanUserRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new BanUserCommand(id, request.Reason), ct);
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}