using MazadZone.Application.Features.Categories.Commands.Restore;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public static class Restore
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/restore", HandleAsync)
           // .RequireAuthorization("AdminPolicy") 
           .WithSummary("Restore a soft-deleted category")
           .WithDescription("Reactivates a previously soft-deleted category, making it visible again. Note: If the category's parent is currently soft-deleted, restoring this child might return a 409 Conflict depending on your domain rules.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
           .ProducesProblem(StatusCodes.Status404NotFound) // Category not found
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule: e.g., Parent is still deleted
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] CategoryId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new RestoreCategoryCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: error => error.ToProblem()
        );
    }
}