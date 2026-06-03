using MazadZone.Application.Features.Orders.Commands.CancelOrder;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public static class Cancel
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/cancel", HandleAsync)
        //    .RequireAuthorization()
           .WithSummary("Cancel an order")
           .WithDescription("Cancels an existing order. This operation is typically restricted by domain rules—for example, an order might only be cancellable if it is still in a 'Pending' state. Returns a 409 Conflict if the order has already been shipped, delivered")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID in the route
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // User is logged in, but lacks permission to cancel THIS specific order
           .ProducesProblem(StatusCodes.Status404NotFound) // Order does not exist
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule violations (e.g., too late to cancel)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] OrderId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new CancelOrderCommand(id), ct);
        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: e => e.ToProblem());
    }
}