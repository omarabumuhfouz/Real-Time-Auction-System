using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Bidders.Queries.GetMyBids;
using MazadZone.Domain.Bidders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MazadZone.Api.Endpoints.Bidders;

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
        [FromQuery] UserId bidderId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? query,
        [FromQuery] Guid? categoryId,
        [FromQuery] string? tab,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var getMyBidsQuery = new GetMyBidsQuery(
            bidderId,
            page ?? 1,
            pageSize ?? 12,
            query,
            categoryId,
            tab ?? "all",
            sortBy ?? "EndTime",
            sortDirection ?? "desc");

        var result = await sender.Send(getMyBidsQuery, ct);
        return result.Match(onValue: value => Results.Ok(value), onError: e => e.ToProblem());
    }
}
