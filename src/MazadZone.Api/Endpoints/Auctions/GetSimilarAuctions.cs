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
            .Produces<IReadOnlyList<AuctionsListDto>>(StatusCodes.Status200OK);
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
