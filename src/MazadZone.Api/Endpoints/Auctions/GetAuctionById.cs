using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Auctions.Queries.GetAuctionById;
using MazadZone.Domain.Auctions;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public static class GetAuctionById
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{auctionId}", HandleAsync)
            .WithName("GetAuctionById")
            .WithOpenApi()
            .WithSummary("Gets auction details by id")
            .Produces<AuctionDto>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid auctionId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetAuctionByIdQuery(AuctionId.From(auctionId));
        var result = await sender.Send(query, ct);
        return result.Match(onValue: value => Results.Ok(value), onError: e => e.ToProblem());
    }
}
