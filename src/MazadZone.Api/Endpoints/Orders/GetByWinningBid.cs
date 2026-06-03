using MazadZone.Application.Features.Orders.Queries.GetOrderByWinningBid;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Domain.Auctions;

namespace MazadZone.Api.Endpoints.Orders;

public static class GetByWinningBid
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/by-bid/{bidId:guid}", HandleAsync)
           // .RequireAuthorization() // Highly recommended: You likely only want the buyer, seller, or admin to view these details
           .WithName("GetOrderByWinningBid")
           .WithSummary("Retrieve an order by winning bid")
           .WithDescription("Fetches the complete details of an order associated with a specific winning bid ID. Returns a 404 Not Found if the bid does not exist or if no order has been created for it yet.")
           .Produces<OrderDetailsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID in the route
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but user is not authorized to view THIS order
           .ProducesProblem(StatusCodes.Status404NotFound) // Order/Bid does not exist
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