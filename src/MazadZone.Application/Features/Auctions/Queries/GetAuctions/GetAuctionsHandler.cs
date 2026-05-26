using System.Reflection.PortableExecutable;
using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Auctions.Queries.GetAuctions;

public class GetAuctionsHandler
(IAuctionQueries _auctionQueries, ILogger<GetAuctionsHandler> _logger)
: IQueryHandler<GetAuctionsQuery, PagedList<AuctionsListDto>>
{
    public async Task<Result<PagedList<AuctionsListDto>>> Handle(GetAuctionsQuery query, CancellationToken ct)
    {
        _logger.LogHandlingGetAuctions(query.SearchTerm, query.CategoryId?.Value, query.CurrentBidAmount, query.SortBy, query.SortDirection);

        var queryParameters = new AuctionQueryParameters
        {
            Page = query.Page,
            PageSize = query.PageSize,
            SearchTerm = query.SearchTerm,
            CategoryId = query.CategoryId,
            SubCategoryId = query.SubCategoryId,
            CurrentBidAmount = query.CurrentBidAmount,
            Status = query.Status,
            SortBy = query.SortBy,
            SortDirection = query.SortDirection
        };

        var auctions = await _auctionQueries.SearchAuctionsAsync(queryParameters, ct);

        if (auctions == null)
        {
            _logger.LogNoAuctionsFound(query.SearchTerm, query.CategoryId?.Value, query.CurrentBidAmount, query.SortBy, query.SortDirection);
            return Result.Failure<PagedList<AuctionsListDto>>(AuctionErrors.NotFound);
        }

        _logger.SuccessRetrievedAuctions(query.SearchTerm, query.CategoryId?.Value, query.CurrentBidAmount, query.SortBy, query.SortDirection, auctions.TotalCount);
        return Result.Success(auctions);

    }
}