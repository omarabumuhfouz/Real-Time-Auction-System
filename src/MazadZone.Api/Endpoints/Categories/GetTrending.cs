using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetTrendingCategories;

namespace MazadZone.Api.Endpoints.Categories;

internal sealed record GetTrendingRequest(int Limit = 10);

public static class GetTrending
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/trending", HandleAsync)
           .AllowAnonymous()
           .WithSummary("Retrieve trending categories")
           .WithDescription("Fetches a list of the most active or popular categories based on current auction activity. Use the 'limit' query parameter to control how many results are returned (defaults to 10).")
           .Produces<IReadOnlyList<TrendingCategoryResponse>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // If 'limit' is an invalid type
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody]GetTrendingRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)

    {
        var result = await sender.Send(new GetTrendingCategoriesQuery(request.Limit), ct);

        return result.Match(
            onValue: trending => Results.Ok(trending),
            onError: error => error.ToProblem()
        );
    }
}