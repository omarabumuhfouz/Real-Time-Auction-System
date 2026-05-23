using MazadZone.Application.Features.Orders.Commands.OpenDispute;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public record OpenDisputeRequest(string Reason)
{
    public OpenDisputeCommand ToCommand(OrderId orderId) => new(orderId, Reason);
}

public static class OpenDispute
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/dispute", HandleAsync)
           // .RequireAuthorization() // Highly recommended: Only the buyer (or sometimes the seller) of this specific order should be able to open a dispute
           .WithSummary("Open a dispute for an order")
           .WithDescription("Initiates a formal dispute for an order, which typically pauses any pending payouts and flags the transaction for administrative review. A valid reason must be provided in the request body. Returns a 409 Conflict if the order is already disputed, or if it is in a state that cannot be disputed (e.g., canceled).")
           .Accepts<OpenDisputeRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID, or if the 'Reason' is empty/too long
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user is not the buyer/seller of THIS order
           .ProducesProblem(StatusCodes.Status404NotFound) // Order does not exist
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule violations (e.g., order is already in a disputed state)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]OrderId id,
        [FromBody] OpenDisputeRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request.ToCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }
}