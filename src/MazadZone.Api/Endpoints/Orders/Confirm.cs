using MazadZone.Application.Features.Orders.Commands.Confirm;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public static class Confirm
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/confirm", HandleAsync)
           .RequireAuthorization(Policies.SellerOnly)
           .WithOpenApi()
           .WithSummary("Confirm an order")
           .WithDescription("Confirms an existing order, transitioning its state from 'Pending'. Indicates the seller has acknowledged the order and is preparing to fulfill it. Returns a 409 Conflict if the order is already confirmed, canceled, or in an invalid state. **Requires Seller role.**")
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
        var result = await sender.Send(new ConfirmOrderCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }
}