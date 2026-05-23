using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Features.Users.Commands.ChangeEmail;

namespace MazadZone.Api.Endpoints.Users;

public record ChangeEmailRequest(string NewEmail);

public static class ChangeEmail
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/email", HandleAsync)
        //    .RequireAuthorization() 
           .WithSummary("Change user email")
           .WithDescription("Updates the email address of the currently authenticated user. Returns a 409 Conflict if the requested new email address is already registered to another account.")
           .Accepts<ChangeEmailRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // e.g., Invalid email format
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status404NotFound) // Bound user no longer exists in the DB
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule: Email is already taken
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ChangeEmailRequest request,
        BoundUserId boundUserId,
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ChangeEmailCommand(boundUserId.Value, request.NewEmail), ct);
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}