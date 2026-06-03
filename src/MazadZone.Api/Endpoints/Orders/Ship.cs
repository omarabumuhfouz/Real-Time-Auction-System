using MazadZone.Application.Features.Orders.Commands.ShipOrder;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public static class Ship
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/ship", HandleAsync)
           .RequireAuthorization()
           .WithSummary("Mark an order as shipped")
           .WithDescription("Updates the status of an order to 'Shipped', indicating that the seller has dispatched the item to the buyer. Returns a 409 Conflict if the order is already shipped, canceled, or hasn't been confirmed/processed yet.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID in the route
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user lacks permission to modify THIS order
           .ProducesProblem(StatusCodes.Status404NotFound) // Order does not exist
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule violations (e.g., invalid state transition)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]OrderId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ShipOrderCommand(id), ct);
        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: e => e.ToProblem());
    }
}