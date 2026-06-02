using MazadZone.Application.Features.Disputes.Queries.GetOpenDisputesBreakdown;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class GetDisputesBreakdown
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/disputes/breakdown", HandleAsync)
           // .RequireAuthorization("AdminOnly") 
           .WithSummary("Get Open Disputes Breakdown")
           .WithDescription("Retrieves the total number of open disputes grouped by type, calculating the percentage change versus the previous period.")
           .Produces<OpenDisputesBreakdownDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,

        [FromServices] ISender sender,
        CancellationToken ct,
        [FromQuery] int limit = 5,
        [FromQuery] bool includeOther = true)
    {
        var result = await sender.Send(new GetOpenDisputesBreakdownQuery(startDate, endDate, limit, includeOther), ct);

        return result.Match(
            stats => Results.Ok(stats),
            error => error.ToProblem());
    }
}