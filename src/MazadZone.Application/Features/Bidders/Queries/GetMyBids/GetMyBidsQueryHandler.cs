using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Bidders;
using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Bidders.Queries.GetMyBids;

public sealed class GetMyBidsQueryHandler : IQueryHandler<GetMyBidsQuery, PagedList<MyBidAuctionDto>>
{
    private readonly IAuctionQueries _auctionQueries;
    private readonly ILogger<GetMyBidsQueryHandler> _logger;

    public GetMyBidsQueryHandler(
        IAuctionQueries auctionQueries,
        ILogger<GetMyBidsQueryHandler> logger)
    {
        _auctionQueries = auctionQueries;
        _logger = logger;
    }

    public async Task<Result<PagedList<MyBidAuctionDto>>> Handle(GetMyBidsQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Handling GetMyBidsQuery for bidder {BidderId}, tab {Tab}, searchTerm {SearchTerm}, category {CategoryId}, sort {SortBy} {SortDirection}.",
            request.BidderId, request.Tab, request.SearchTerm, request.CategoryId, request.SortBy, request.SortDirection);

        var parameters = new MyBidsQueryParameters
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SearchTerm = request.SearchTerm,
            CategoryId = request.CategoryId,
            Tab = request.Tab,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection
        };

        var bids = await _auctionQueries.SearchMyBidsAsync(request.BidderId, parameters, ct);
        return Result.Success(bids);
    }
}
