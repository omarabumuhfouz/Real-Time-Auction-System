using MazadZone.Application.Features.Categories.Commands.Create;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public record CreateCategoryRequest(
    string Name, 
    string Description, 
    CategoryId? ParentId);

public static class Create
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
            .RequireAuthorization("AdminPolicy") // restricting category creation to admins
            .WithSummary("Create a new auction category")
            .WithDescription("Creates a new category for auctions. To create a root category, omit the ParentId. To create a sub-category, provide a valid existing ParentId. Returns the newly generated category ID.")
            .Accepts<CreateCategoryRequest>("application/json")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Validation failures
            .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
            .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
            .ProducesProblem(StatusCodes.Status404NotFound) // Parent category missing
            .ProducesProblem(StatusCodes.Status409Conflict) // Name uniqueness or domain rules
            .ProducesProblem(StatusCodes.Status500InternalServerError); // Infrastructure/Server errors
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateCategoryRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description,
            request.ParentId
        );

        var result = await sender.Send(command, ct);

        return result.Match(
            onValue: id => Results.Created($"/api/v1/categories/{id}", id),
            onError: error => error.ToProblem() 
        );
    }
}