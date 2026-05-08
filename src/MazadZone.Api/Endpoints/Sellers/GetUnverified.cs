using MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;

namespace MazadZone.Api.Endpoints.Sellers;

public static class GetUnverified
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("unverified", GetUnverifiedSellersAsync)
           .WithSummary("Retrieves all unverified sellers")
           .Produces<IReadOnlyList<UnverifiedSellerSummaryResponse>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetUnverifiedSellersAsync(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetUnverifiedSellersQuery(), ct);
        return result.Match(sellers => Results.Ok(sellers), e => e.ToProblem());
    }
}