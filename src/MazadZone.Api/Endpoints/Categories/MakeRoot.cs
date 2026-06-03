using MazadZone.Application.Features.Categories.Commands.MakeRootCategory;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;
public static class MakeRoot
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/make-root", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Promote a sub-category to a root category")
           .WithDescription("Detaches a sub-category from its parent, elevating it to a top-level (root) category. If the category is already a root category, this operation might return a 409 Conflict depending on your domain rules.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict) // Added for domain rule violations
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]CategoryId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new MakeRootCategoryCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: error => error.ToProblem()
        );
    }
}