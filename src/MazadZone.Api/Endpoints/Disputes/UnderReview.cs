using MazadZone.Application.Features.Orders.Commands.ResolveDispute; // Keep or change based on your new command namespace
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Disputes;

public static class UnderReview
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/under-review", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithOpenApi()
           .WithSummary("Move a dispute to 'Under Review'")
           .WithDescription("Updates the status of an active dispute to 'Under Review', indicating an administrator is actively investigating it. Returns a 409 Conflict if the dispute is already Resolved. **Requires Admin role.**")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict)
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