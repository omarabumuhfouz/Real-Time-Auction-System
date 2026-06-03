using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Auctions.Queries;
using MazadZone.Application.Features.Auctions.Queries.GetSimilarAuctions;
using MazadZone.Domain.Auctions;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public static class GetSimilarAuctions
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{auctionId}/similar", HandleAsync)
            .WithName("GetSimilarAuctions")
            .WithOpenApi()
            .WithSummary("Gets similar auctions")
            .WithDescription("Retrieves a list of auctions similar to the specified one (same category). Use the optional 'limit' parameter to control how many results are returned (default: 6). This is a public endpoint — no authentication required.")
            .Produces<IReadOnlyList<AuctionsListDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] AuctionId auctionId,
        [FromQuery] int? limit,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetSimilarAuctionsQuery(auctionId, limit.GetValueOrDefault(6));
        var result = await sender.Send(query, ct);
        return result.Match(onValue: value => Results.Ok(value), onError: e => e.ToProblem());
    }
}
