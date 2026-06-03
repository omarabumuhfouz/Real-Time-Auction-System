using MazadZone.Application.Features.Categories.Commands.Delete;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public static class Delete
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", HandleAsync)
            .RequireAuthorization(Policies.AdminOnly) 
            .WithSummary("Delete a category")
            .WithDescription("Deletes a specific category and automatically cascades the deletion to its subcategories. This operation will fail with a 409 Conflict if the category, or any of its nested subcategories, are currently linked to active auctions.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Malformed route ID
            .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
            .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
            .ProducesProblem(StatusCodes.Status404NotFound) // Category does not exist
            .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule: e.g., Active Auctions exist
            .ProducesProblem(StatusCodes.Status500InternalServerError);

    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] CategoryId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: error => error.ToProblem() 
        );
    }
}