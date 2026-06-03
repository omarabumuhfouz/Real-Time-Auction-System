using MazadZone.Application.Features.Orders.Queries.GetOrderDetails;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public static class GetDetails
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
           .RequireAuthorization(Policies.Shared)
           .WithOpenApi()
           .WithName("GetOrderDetails")
           .WithSummary("Retrieve order details by ID")
           .WithDescription("Fetches the complete details of a specific order, including the receipt address, financial amount, and current status. Access is restricted to authenticated users (buyer, seller, or admin). Returns a 404 Not Found if the order does not exist. **Requires authentication (any role).**")
           .Produces<OrderDetailsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
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