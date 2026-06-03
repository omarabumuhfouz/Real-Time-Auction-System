using MazadZone.Application.Features.Dashboard.Queries.GetAuctionActivityTrends;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class GetAuctionActivityTrends
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions/trends", HandleAsync)
           // .RequireAuthorization("AdminOnly") 
           .WithSummary("Get Auction & Bidding Trends (Chart Data)")
           .WithDescription("Retrieves totals, growth percentages, and grouped data points for rendering the Auction Activity dual-axis chart.")
           .Produces<AuctionActivityTrendsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string period, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetAuctionActivityTrendsQuery(startDate, endDate, period), ct);

        return result.Match(
            stats => Results.Ok(stats),
            error => error.ToProblem());
    }
}