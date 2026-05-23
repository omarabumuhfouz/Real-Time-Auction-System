using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetSubCategories;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public static class GetSub
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/sub-categories", HandleAsync)
           .AllowAnonymous() 
           .WithSummary("Retrieve direct sub-categories for a parent category")
           .WithDescription("Fetches a list of all immediate child categories for the specified parent category ID. This does not return deeply nested descendants. Returns a 404 if the parent category does not exist.")
           .Produces<IReadOnlyList<CategoryResponse>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Malformed GUID in route
           .ProducesProblem(StatusCodes.Status404NotFound) // Parent category not found
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] CategoryId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetSubCategoriesQuery(id), ct);

        return result.Match(
            onValue: subCategories => Results.Ok(subCategories),
            onError: error => error.ToProblem()
        );
    }
}