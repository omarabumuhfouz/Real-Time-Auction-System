using MazadZone.Application.Features.Users.Commands.Suspend;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Api.Endpoints.Users;

public record SuspendUserRequest(string Reason,DateTime Until);

public static class Suspend
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/suspend", HandleAsync)
        //    .RequireAuthorization("AdminOnly")
           .WithSummary("Suspend a user account temporarily")
           .WithDescription("Temporarily suspends a user account until the specified 'Until' date, preventing them from logging in or participating in auctions. Both a reason and a future date must be provided. Returns a 409 Conflict if the user is already suspended or permanently banned.")
           .Accepts<SuspendUserRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // e.g., 'Until' date is in the past, or 'Reason' is empty
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user lacks the AdminOnly policy
           .ProducesProblem(StatusCodes.Status404NotFound) // User does not exist
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]UserId id,
        [FromBody] SuspendUserRequest request,
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new SuspendUserCommand(id, request.Reason ,request.Until), ct);
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}