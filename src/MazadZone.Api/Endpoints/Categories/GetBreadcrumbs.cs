using MazadZone.Application.Features.Categories.Queries;
using MazadZone.Application.Features.Categories.Queries.GetBreadcrumbs;
using MazadZone.Domain.Categories;

namespace MazadZone.Api.Endpoints.Categories;

public static class GetBreadcrumbs
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/breadcrumbs", HandleAsync)
           .AllowAnonymous() // Assuming category navigation is publicly visible
           .WithSummary("Retrieve category breadcrumbs")
           .WithDescription("Retrieves the ordered hierarchical path from the root ancestor down to the specified category. This is optimized for rendering UI navigation trails.")
           .Produces<IReadOnlyList<BreadcrumbResponse>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] CategoryId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetCategoryBreadcrumbsQuery(id), ct);

        return result.Match(
            onValue: breadcrumbs => Results.Ok(breadcrumbs),
            onError: error => error.ToProblem()
        );
    }
}