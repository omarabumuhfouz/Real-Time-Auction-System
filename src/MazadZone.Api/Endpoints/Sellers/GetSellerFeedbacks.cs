using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Application.Features.Sellers.Queries.GetFeedbacks;

namespace MazadZone.Api.Endpoints.Sellers;

public static class GetSellerFeedbacks
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/feedbacks", HandleAsync)
           .AllowAnonymous() 
           .WithSummary("Retrieve paginated feedbacks for a seller")
           .WithDescription("Fetches a paginated list of public feedbacks, comments, and ratings left by buyers for a specific seller. Safe to display to unauthenticated users.")
           .Produces<PagedList<FeedbackDto>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Malformed GUID or invalid pagination inputs
           .ProducesProblem(StatusCodes.Status404NotFound) // Seller not found
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UserId id,
        [FromServices] ISender sender,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await sender.Send(new GetSellerFeedbacksQuery(id, page, pageSize), ct);
        
        return result.Match(
            response => Results.Ok(response),
            e => e.ToProblem());
    }
}