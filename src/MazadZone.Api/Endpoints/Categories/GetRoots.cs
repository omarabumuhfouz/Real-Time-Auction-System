using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetRootCategories;

namespace MazadZone.Api.Endpoints.Categories;

public static class GetRoots
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/roots", HandleAsync)
           .AllowAnonymous() // Typically, main categories are visible to public guests
           .WithSummary("Retrieve all root categories")
           .WithDescription("Fetches all top-level categories (categories that do not have a parent). This endpoint is optimized for populating main navigation menus and homepage directory lists.")
           .Produces<IReadOnlyList<CategoryResponse>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetRootCategoriesQuery(), ct);

        return result.Match(
            onValue: categories => Results.Ok(categories),
            onError: error => error.ToProblem()
        );
    }
}