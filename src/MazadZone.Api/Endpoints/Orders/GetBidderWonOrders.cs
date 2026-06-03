using MazadZone.Api.Constants;
using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.GetBidderWonOrders;

namespace MazadZone.Api.Endpoints.Orders;

public static class GetBidderWonOrders
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/won", HandleAsync)
           .RequireAuthorization(Policies.BidderOnly)
           .WithSummary("Get paginated won orders for the Bidder")
           .WithDescription("Retrieves a paginated list of auctions the authenticated bidder has won. Supports filtering by status (e.g., 'Pending', 'Shipped', 'All').")
           .Produces<PagedList<WonOrderSummaryDto>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        // BoundUserId boundUserId,
        UserId bidderId ,
        [FromServices] ISender sender,
        CancellationToken ct,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await sender.Send(new GetBidderWonOrdersQuery(bidderId, status, page, pageSize), ct);

        return result.Match(
            data => Results.Ok(data),
            error => error.ToProblem());
    }
}