using MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class GetUserGrowthTrends
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/growth", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly) 
           .WithSummary("Get User Growth Trends (Chart Data)")
           .WithDescription("Retrieves total new users/sellers, growth percentages, and grouped data points for rendering line charts.")
           .Produces<UserGrowthTrendsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string period, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetUserGrowthTrendsQuery(startDate, endDate, period), ct);

        return result.Match(
            stats => Results.Ok(stats),
            error => error.ToProblem());
    }
}