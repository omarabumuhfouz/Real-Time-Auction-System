using MazadZone.Application.Features.Orders.Commands.Confirm;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public static class Confirm
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/confirm", HandleAsync)
           // .RequireAuthorization() // for Admain or Seller
           .WithSummary("Confirm an order")
           .WithDescription("Confirms an existing order, typically transitioning its state from 'Pending'. This indicates the seller has acknowledged the order and is preparing to fulfill it. Returns a 409 Conflict if the order is already confirmed, canceled, or in an invalid state for confirmation.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID in the route
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // User is logged in, but lacks permission to confirm THIS specific order
           .ProducesProblem(StatusCodes.Status404NotFound) // Order does not exist
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule violations (e.g., order is already confirmed)
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