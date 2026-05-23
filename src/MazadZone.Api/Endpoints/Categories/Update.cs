using MazadZone.Application.Features.Categories.Commands.Update;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public record UpdateCategoryRequest(string Name, string Description);

public static class Update
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", HandleAsync)
        //    .RequireAuthorization("AdminPolicy")
           .WithSummary("Update category details")
           .WithDescription("Updates the name and description of an existing category. Returns a 409 Conflict if the new name is already taken by another category in the same hierarchy level, or if it violates other domain uniqueness rules.")
           .Accepts<UpdateCategoryRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For payload validation failures
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
           .ProducesProblem(StatusCodes.Status404NotFound) // If the CategoryId doesn't exist
           .ProducesProblem(StatusCodes.Status409Conflict) // For domain rule violations (e.g., duplicate names)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] CategoryId id,
        [FromBody] UpdateCategoryRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        // Mapping the Request + Route ID to the Application Command
        var command = new UpdateCategoryCommand(
            id, 
            request.Name, 
            request.Description);

        var result = await sender.Send(command, ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: error => error.ToProblem()
        );
    }
}