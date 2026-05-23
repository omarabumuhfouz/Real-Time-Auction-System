using MazadZone.Application.Features.Orders.Queries.GetOrderDetails;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public static class GetDetails
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
           // .RequireAuthorization() // Highly recommended: Orders contain sensitive PII (addresses) and financial data
           .WithName("GetOrderDetails")
           .WithSummary("Retrieve order details by ID")
           .WithDescription("Fetches the complete details of a specific order, including the receipt address, financial amount, and current status. Access is typically restricted to the buyer, the seller, or system administrators. Returns a 404 Not Found if the order does not exist.")
           .Produces<OrderDetailsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID in the route
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user is not authorized to view THIS order
           .ProducesProblem(StatusCodes.Status404NotFound) // Order does not exist
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]OrderId id,
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetOrderDetailsQuery(id), ct);

        return result.Match(
           onValue: value => Results.Ok(value),
           onError: e => e.ToProblem());
    }
}