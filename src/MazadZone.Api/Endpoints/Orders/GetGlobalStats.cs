using MazadZone.Application.Features.Orders.Queries.GetGlobalStats;
using MazadZone.Application.Features.Orders.Queries.DTOs;

namespace MazadZone.Api.Endpoints.Orders;

public static class GetGlobalStats
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/stats/global", HandleAsync)
        //    .RequireAuthorization("AdminOnly")
           .WithName("GetGlobalStats")
           .WithSummary("Retrieve global platform statistics")
           .WithDescription("Fetches high-level, aggregate statistics across all orders on the platform (e.g., total volume, revenue, completed vs. canceled ratios). Because this exposes system-wide financial and operational metrics, access is strictly limited to system administrators.")
           .Produces<AdminGlobalStatsDto>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user lacks the AdminOnly policy
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetGlobalStatsQuery(), ct);
        return result.Match(
            onValue: value => Results.Ok(value),
            onError: e => e.ToProblem()
        );
    }
}