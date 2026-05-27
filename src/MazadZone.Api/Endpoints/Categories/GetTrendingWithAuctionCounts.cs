using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetTrendingCategoriesWithAuctionCount;

namespace MazadZone.Api.Endpoints.Categories;

internal sealed record GetTrendingAuctionsRequest(int Limit = 10);

public static class GetTrendingWithAuctionCounts
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/categories/trending-auctions", HandleAsync)
           .AllowAnonymous()
           .WithSummary("Retrieve trending categories item counts")
           .WithDescription("Fetches high-activity product categories sorted dynamically by unique live auction instances running inside them over the past 24 hours.")
           .Produces<IReadOnlyList<TrendingCategoryAuctionCountResponse>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetTrendingAuctionsRequest request, // Fixed: Changed from [FromBody] for standard GET requests
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetTrendingCategoriesWithAuctionCountQuery(request.Limit), ct);

        return result.Match(
            onValue: data => Results.Ok(data),
            onError: error => error.ToProblem()
        );
    }
}