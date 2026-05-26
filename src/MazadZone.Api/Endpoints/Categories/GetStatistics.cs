using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetCategoryStatistics;

namespace MazadZone.Api.Endpoints.Categories;

public static class GetStatistics
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/statistics", HandleAsync)
            // .RequireAuthorization("AdminPolicy")
            .WithSummary("Retrieve category statistics")
            .WithDescription("Retrieves aggregated analytical data across all categories, such as the total number of active auctions, total items sold, or engagement metrics. Useful for building dashboards and reporting features.")
            .Produces<IReadOnlyList<CategoryStatResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
            .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetCategoryStatisticsQuery(), ct);

        return result.Match(
            onValue: stats => Results.Ok(stats),
            onError: error => error.ToProblem()
        );
    }
}