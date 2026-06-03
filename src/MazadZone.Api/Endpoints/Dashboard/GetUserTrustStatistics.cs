using MazadZone.Application.Features.Users.Queries.GetUserTrustStats;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class GetUserTrustStatistics
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/user-trust", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly) 
           .WithSummary("Get User Trust Dashboard metrics")
           .WithDescription("Retrieves aggregate totals, account status percentages, and overall trust score for a specific period (excludes Admins).")
           .Produces<UserTrustStatsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetUserTrustStatsQuery(startDate, endDate), ct);

        return result.Match(
            stats => Results.Ok(stats),
            error => error.ToProblem());
    }
}