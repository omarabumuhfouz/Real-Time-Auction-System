using MazadZone.Api.Constants;
using MazadZone.Application.Features.Orders.Commands.ResolveDispute;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Disputes;

public record ResolveDisputeRequest(string Resolution)
{
    public ResolveDisputeCommand ToCommand(DisputeId disputeId) => new(disputeId, Resolution);
}

public static class ResolveDispute
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/resolve", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithOpenApi()
           .WithSummary("Resolve an open dispute")
           .WithDescription("Closes an active dispute on an order, allowing payouts to proceed or initiating a refund. A formal resolution note must be provided. Returns a 409 Conflict if the order is not currently disputed or if the dispute has already been resolved. **Requires Admin role.**")
           .Accepts<ResolveDisputeRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]DisputeId id,
        [FromBody] ResolveDisputeRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request.ToCommand(id), ct);
        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: e => e.ToProblem());
    }
}