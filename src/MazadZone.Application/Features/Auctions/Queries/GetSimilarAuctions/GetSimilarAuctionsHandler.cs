
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;


namespace MazadZone.Application.Features.Auctions.Queries.GetSimilarAuctions;

public class GetSimilarAuctionsHandler(
    IAuctionQueries _auctionQueries,
    ILogger<GetSimilarAuctionsHandler> _logger)
    : IQueryHandler<GetSimilarAuctionsQuery, IReadOnlyList<AuctionsListDto>>
{
    public async Task<Result<IReadOnlyList<AuctionsListDto>>> Handle(GetSimilarAuctionsQuery request, CancellationToken ct)
    {
        _logger.LogHandlingGetSimilarAuctions(request.AuctionId.Value, request.Limit);

        var similarAuctions = await _auctionQueries.GetSimilarAuctionsAsync(request.AuctionId.Value, request.Limit, ct);

        if (similarAuctions == null || similarAuctions.Count == 0)
        {
            _logger.LogNoSimilarAuctionsFound(request.AuctionId.Value);
            return Result.Failure<IReadOnlyList<AuctionsListDto>>(AuctionErrors.NotFound);
        }

        _logger.LogSimilarAuctionsSuccess(request.AuctionId.Value, similarAuctions.Count);
        return Result.Success(similarAuctions);
    }
}
