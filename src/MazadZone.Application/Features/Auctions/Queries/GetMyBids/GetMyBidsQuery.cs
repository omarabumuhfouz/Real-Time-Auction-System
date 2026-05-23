using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Common.Messaging;
using MazadZone.Domain.Bidders;

namespace MazadZone.Application.Features.Auctions.Queries.GetMyBids;

public sealed record GetMyBidsQuery(
    BidderId BidderId,
    int Page = 1,
    int PageSize = 12,
    string? SearchTerm = null,
    Guid? CategoryId = null,
    string Tab = "all",
    string SortBy = "EndTime",
    string SortDirection = "desc"
) : ICachedQuery<PagedList<MyBidAuctionDto>>
{
    public string CacheKey =>
        $"my-bids:bidder={BidderId.Value}:p={Page}:ps={PageSize}" +
        $":q={SearchTerm ?? "all"}" +
        $":cat={CategoryId?.ToString() ?? "all"}" +
        $":tab={Tab.ToLowerInvariant()}" +
        $":sort={SortBy}:{SortDirection}";

    public string[] Tags => ["auctions", "bids"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(2);
}
