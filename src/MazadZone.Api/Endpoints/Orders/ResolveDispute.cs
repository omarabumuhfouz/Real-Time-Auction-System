using MazadZone.Application.Features.Orders.Commands.ResolveDispute;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public record ResolveDisputeRequest(string Resolution)
{
    public ResolveDisputeCommand ToCommand(OrderId orderId) => new(orderId, Resolution);
}

public static class ResolveDispute
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/dispute/resolve", HandleAsync)
           // .RequireAuthorization("AdminOnly") // Highly recommended: Typically, only platform administrators or support staff resolve disputes
           .WithSummary("Resolve an open dispute")
           .WithDescription("Closes an active dispute on an order, typically allowing payouts to proceed or initiating a refund. A formal resolution note must be provided. Returns a 409 Conflict if the order is not currently disputed or if the dispute has already been resolved.")
           .Accepts<ResolveDisputeRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID, or if the 'Resolution' is empty/too long
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user lacks permission (e.g., not an Admin)
           .ProducesProblem(StatusCodes.Status404NotFound) // Order does not exist
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule violations (e.g., no open dispute exists for this order)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]OrderId id,
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