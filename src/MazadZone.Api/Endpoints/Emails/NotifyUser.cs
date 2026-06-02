using MazadZone.Application.Features.Emails.Commands.NotifyUser;

namespace MazadZone.Api.Endpoints.Emails;

public static class NotifyUser
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/notify", HandleAsync)
            // .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Send an email notification to a user")
           .WithDescription("Sends an email using the standard system HTML template. Requires a valid UserId, Title, and Message.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) 
           .ProducesProblem(StatusCodes.Status401Unauthorized) 
           .ProducesProblem(StatusCodes.Status403Forbidden) 
           .ProducesProblem(StatusCodes.Status404NotFound) 
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] NotifyUserRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        // Pass both Title and Message to the MediatR command
        var command = new NotifyUserCommand(request.UserId, request.Title, request.Message);
        
        var result = await sender.Send(command, ct);
        
        return result.Match(
            onValue: _ => Results.NoContent(), 
            onError: e => e.ToProblem());
    }
}

// DTO representing the HTTP JSON body
public record NotifyUserRequest(UserId UserId, string Title, string Message);