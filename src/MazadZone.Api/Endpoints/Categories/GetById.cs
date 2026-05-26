using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetById;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public static class GetById
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
           .AllowAnonymous() // Assuming categories are publicly viewable
           .WithSummary("Retrieve a category by ID")
           .WithDescription("Fetches the detailed information for a specific category using its unique identifier. Returns a 404 Not Found if the category does not exist.")
           .Produces<CategoryResponse>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Malformed GUID in route
           .ProducesProblem(StatusCodes.Status404NotFound) // Category not found
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] CategoryId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id), ct);

        return result.Match(
            onValue: category => Results.Ok(category),
            onError: error => error.ToProblem()
        );
    }
}