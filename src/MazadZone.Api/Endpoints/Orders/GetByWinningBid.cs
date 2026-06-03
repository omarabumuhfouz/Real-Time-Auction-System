using MazadZone.Application.Features.Orders.Queries.GetOrderByWinningBid;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Domain.Auctions;

namespace MazadZone.Api.Endpoints.Orders;

public static class GetByWinningBid
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/by-bid/{bidId:guid}", HandleAsync)
           .RequireAuthorization(Policies.Shared)
           .WithOpenApi()
           .WithName("GetOrderByWinningBid")
           .WithSummary("Retrieve an order by winning bid")
           .WithDescription("Fetches the complete details of an order associated with a specific winning bid ID. Returns a 404 Not Found if the bid does not exist or if no order has been created for it yet. **Requires authentication (any role).**")
           .Produces<OrderDetailsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]BidId bidId,
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetOrderByWinningBidQuery(bidId), ct);
        return result.Match(
            onValue: value => Results.Ok(value),
            onError: e => e.ToProblem());
    }
}