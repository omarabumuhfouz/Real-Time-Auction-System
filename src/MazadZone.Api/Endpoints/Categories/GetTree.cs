using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetCategoryTree;

namespace MazadZone.Api.Endpoints.Categories;

public static class GetTree
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/tree", HandleAsync)
           .AllowAnonymous() 
           .WithSummary("Retrieve the full hierarchical category tree")
           .WithDescription("Fetches the complete, deeply nested category structure including all root categories and their descendants. This endpoint is ideal for rendering complete multi-level navigation menus or caching the entire category structure on the client side.")
           .Produces<IReadOnlyList<CategoryTreeResponse>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetCategoryTreeQuery(), ct);

        return result.Match(
            onValue: tree => Results.Ok(tree),
            onError: error => error.ToProblem()
        );
    }
}