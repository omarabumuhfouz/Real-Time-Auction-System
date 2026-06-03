using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetTrendingRootCategories;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class GetTrendingRootCategories
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/categories/root/trending", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly) 
           .WithSummary("Get Trending Root Categories")
           .WithDescription("Retrieves statistics exclusively for top-level (root) categories based on active auction counts. Optionally bundles all remaining root categories into a single 'Other' bucket if the limit is exceeded.")
           .Produces<IReadOnlyList<CategoryStatResponse>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) 
           .ProducesValidationProblem(StatusCodes.Status403Forbidden)
           .ProducesValidationProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken ct,
        [FromQuery] int limit = 5,
        [FromQuery] bool includeOther = true)
    {
        var result = await sender.Send(new GetTrendingRootCategoriesQuery(limit, includeOther), ct);

        return result.Match(
            stats => Results.Ok(stats),
            error => error.ToProblem());
    }
}