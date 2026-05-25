using MazadZone.Application.Features.Orders.Queries.GetSellerStats;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Domain.Sellers;

namespace MazadZone.Api.Endpoints.Orders;

public static class GetSellerStats
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/stats/seller/{sellerId:guid}", HandleAsync)
           // .RequireAuthorization() // Highly recommended: A seller's financial and operational statistics should typically be private
           .WithName("GetSellerStats")
           .WithSummary("Retrieve order statistics for a specific seller")
           .WithDescription("Fetches aggregated order statistics (e.g., total sales volume, completed vs. canceled orders) for a specific seller profile. Access should generally be restricted to the seller themselves or system administrators. Returns a 404 Not Found if the seller does not exist.")
           .Produces<SellerOrderStatsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID in the route
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user is not authorized to view THIS seller's stats
           .ProducesProblem(StatusCodes.Status404NotFound) // Seller does not exist
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]UserId sellerId,
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetSellerStatsQuery(sellerId), ct);
        return result.Match(
            onValue: value => Results.Ok(value),
            onError: e => e.ToProblem()
        );
    }
}