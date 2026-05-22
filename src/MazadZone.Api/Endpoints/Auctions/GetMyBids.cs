using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Auctions.Queries.GetMyBids;
using MazadZone.Domain.Bidders;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public static class GetMyBids
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/my-bids", HandleAsync)
           .WithName("GetMyBids")
           .WithOpenApi()
           .Produces<PagedList<MyBidAuctionDto>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] BidderId bidderId,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? query,
        [FromQuery] Guid? categoryId,
        [FromQuery] string tab,
        [FromQuery] string sortBy,
        [FromQuery] string sortDirection,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var getMyBidsQuery = new GetMyBidsQuery(
            bidderId,
            page == 0 ? 1 : page,
            pageSize == 0 ? 12 : pageSize,
            query,
            categoryId,
            string.IsNullOrEmpty(tab) ? "all" : tab,
            string.IsNullOrEmpty(sortBy) ? "EndTime" : sortBy,
            string.IsNullOrEmpty(sortDirection) ? "desc" : sortDirection);

        var result = await sender.Send(getMyBidsQuery, ct);
        return result.Match(onValue: value => Results.Ok(value), onError: e => e.ToProblem());
    }
}
