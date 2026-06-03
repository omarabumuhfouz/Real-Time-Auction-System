using MazadZone.Application.Features.Orders.Commands.CancelOrder;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public static class Cancel
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/cancel", HandleAsync)
           .RequireAuthorization(Policies.BidderOnly)
           .WithOpenApi()
           .WithSummary("Cancel an order")
           .WithDescription("Cancels an existing order. Only cancellable if still in a 'Pending' state. Returns a 409 Conflict if the order has already been shipped or delivered. **Requires Bidder role.**")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict)
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