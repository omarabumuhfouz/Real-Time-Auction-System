using MazadZone.Application.Features.Orders.Commands.ResolveDispute; // Keep or change based on your new command namespace
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Disputes;

public static class UnderReview
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/under-review", HandleAsync)
         .RequireAuthorization(Policies.AdminOnly) 
           .WithSummary("Move a dispute to 'Under Review'")
           .WithDescription("Updates the status of an active dispute to 'Under Review' to indicate an administrator is actively investigating it. Returns a 409 Conflict if the dispute is already under Resolved")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Lacks admin permission[]
           .ProducesProblem(StatusCodes.Status404NotFound) // Dispute/Order does not exist
           .ProducesProblem(StatusCodes.Status409Conflict) // State violation (e.g., already Resolved)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] DisputeId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new UnderReviewDisputeCommand(id), ct);
        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: e => e.ToProblem());
    }
}