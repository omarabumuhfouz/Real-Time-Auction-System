using MazadZone.Application.Features.Categories.Commands.AddSubCategory;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public static class AddSubCategory
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{parentId:guid}/sub-categories/{subCategoryId:guid}", HandleAsync)
            .RequireAuthorization(Policies.AdminOnly) 
            .WithSummary("Link a sub-category to a parent category")
            .WithDescription("Establishes a hierarchical relationship by linking an existing sub-category to a parent category. Returns a conflict (409) if the relationship already exists, creates a circular dependency, or violates other domain rules.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
            .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict) 
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] CategoryId parentId,
        [FromRoute] CategoryId subCategoryId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new AddSubCategoryCommand(parentId, subCategoryId), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: error => error.ToProblem() 
        );
    }
}