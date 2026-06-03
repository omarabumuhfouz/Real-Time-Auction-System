using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Bidders.Queries.GetMyBids;
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
           .WithSummary("Get the authenticated bidder's bids")
           .WithDescription("Returns a paginated list of auctions the authenticated bidder has participated in. The bidder identity is taken from the JWT — no BidderId parameter is accepted. **Requires Bidder role.**")
           .RequireAuthorization(Policies.BidderOnly)
           .Produces<PagedList<MyBidAuctionDto>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? query,
        [FromQuery] Guid? categoryId,
        [FromQuery] string? tab,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var getMyBidsQuery = new GetMyBidsQuery(
            boundUserId.Value,
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
