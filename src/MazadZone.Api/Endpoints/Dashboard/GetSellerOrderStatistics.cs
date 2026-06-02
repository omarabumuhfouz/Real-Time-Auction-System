using MazadZone.Application.Features.Orders.Queries.GetSellerOrderStatistics;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class GetSellerOrderStatistics
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/statistics", HandleAsync)
        //    .RequireAuthorization() // Requires the seller to be logged in
           .WithSummary("Get order statistics for the Seller Dashboard")
           .WithDescription("Retrieves the total counts of the authenticated seller's orders grouped by their current status. Used to populate the top summary cards on the dashboard.")
           .Produces<OrderStatisticsDto>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        // BoundUserId boundUserId,
        UserId sellerId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetSellerOrderStatisticsQuery(sellerId), ct);

        return result.Match(
            stats => Results.Ok(stats),
            error => error.ToProblem());
    }
}