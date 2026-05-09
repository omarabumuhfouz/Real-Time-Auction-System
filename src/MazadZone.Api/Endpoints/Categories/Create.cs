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
        app.MapPost("/", CreateCategoryAsync)
           .WithTags("Category Commands")
           .WithSummary("Creates a new auction category")
           .Produces<Guid>(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)        // Validation failures
           .Produces(StatusCodes.Status404NotFound)       // Parent category missing
           .Produces(StatusCodes.Status409Conflict)       // Name uniqueness or domain rules
           .Produces(StatusCodes.Status500InternalServerError); // Infrastructure/Server errors
    }

    private static async Task<IResult> CreateCategoryAsync(
        [FromBody] CreateCategoryRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        // Internal Mapping: Convert the raw Request into a strongly-typed Command
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description,
            request.ParentId
        );

        var result = await sender.Send(command, ct);

        // Map the Application Result to the appropriate HTTP Response
        return result.Match(
            onValue: id => Results.Created($"/api/v1/categories/{id}", id),
            onError: error => error.ToProblem() 
            // The extension ToProblem() handles mapping ErrorType.Conflict to 409, 
            // ErrorType.Validation to 400, and ErrorType.NotFound to 404.
        );
    }
}