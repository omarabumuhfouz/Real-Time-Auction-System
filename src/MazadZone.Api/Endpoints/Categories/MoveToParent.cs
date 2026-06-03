using MazadZone.Application.Features.Categories.Commands.MoveToParent;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public record MoveToParentRequest(CategoryId? NewParentId);

public static class MoveToParent
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/move", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Move a category to a new parent")
           .WithDescription("Relocates a category to a different parent. If 'newParentId' is provided as null, the category will be moved to the root level. This operation will return a 409 Conflict if the move creates a circular dependency (e.g., trying to move a parent into its own child category).")
           .Accepts<MoveToParentRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict) // Explicitly documenting the circular reference domain error
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] CategoryId id,
        [FromBody] MoveToParentRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new MoveToParentCommand(id, request.NewParentId), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: error => error.ToProblem()
        );
    }
}