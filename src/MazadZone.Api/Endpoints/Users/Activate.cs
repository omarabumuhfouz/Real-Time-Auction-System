using MazadZone.Application.Features.Users.Commands.Activate;

namespace MazadZone.Api.Endpoints.Users;

public static class Activate
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/activate", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Activate a user account")
           .WithDescription("Reactivates a previously suspended or inactive user account, restoring their access to the platform. Returns a 409 Conflict if the user is already active.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Malformed GUID
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user is not an Admin
           .ProducesProblem(StatusCodes.Status404NotFound) // User does not exist
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]UserId id,
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ActivateUserCommand(id), ct);
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}