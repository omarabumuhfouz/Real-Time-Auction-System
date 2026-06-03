using MazadZone.Application.Features.Dashboard.DTOs;
using MazadZone.Application.Features.Dashboard.Queries.GetStats;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class GetStatistics
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/statistics", HandleAsync)
           // .RequireAuthorization("AdminOnly") // Uncomment when auth is ready
           .WithSummary("Get dashboard summary statistics")
           .WithDescription("Retrieves totals and percentage changes for the admin dashboard cards based on a date range.")
           .Produces<DashboardStatsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetDashboardStatsQuery(startDate, endDate);
        var result = await sender.Send(query, ct);

        return result.Match(
            stats => Results.Ok(stats),
            error => error.ToProblem());
    }
}