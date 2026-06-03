using MazadZone.Application.Features.Orders.Commands.DeliverOrder;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public static class Deliver
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/deliver", HandleAsync)
           .RequireAuthorization(Policies.SellerOnly)
           .WithOpenApi()
           .WithSummary("Mark an order as delivered")
           .WithDescription("Updates the status of an order to 'Delivered'. This finalizes the transaction and unlocks the buyer's ability to leave feedback. Returns a 409 Conflict if the order is already delivered, has been canceled, or hasn't been shipped yet. **Requires Seller role.**")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]OrderId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeliverOrderCommand(id), ct);
        return result.Match(onValue: _ => Results.NoContent(), onError: e => e.ToProblem());
    }
}