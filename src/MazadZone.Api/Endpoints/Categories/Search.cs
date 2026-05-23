using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.SearchCategories;

namespace MazadZone.Api.Endpoints.Categories;

public record SearchCategoriesRequest(string SearchTerm);

public static class Search
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/search", HandleAsync)
           .AllowAnonymous() 
           .WithSummary("Search categories by name")
           .WithDescription("Performs a partial text search against category names and descriptions. Pass the search query using the 'searchTerm' query parameter (e.g., /categories/search?searchTerm=electronics).")
           .Produces<IReadOnlyList<CategoryResponse>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // If searchTerm fails validation (e.g., too short)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] SearchCategoriesRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new SearchCategoriesQuery(request.SearchTerm), ct);

        return result.Match(
            onValue: results => Results.Ok(results),
            onError: error => error.ToProblem()
        );
    }
}