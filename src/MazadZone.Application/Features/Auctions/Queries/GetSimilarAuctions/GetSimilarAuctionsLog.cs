using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Auctions.Queries.GetSimilarAuctions;

public static partial class GetSimilarAuctionsLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.GetSimilarAuctionsAttempt, Level = LogLevel.Information, Message = "Handling GetSimilarAuctionsQuery for auction {AuctionId} with limit {Limit}.")]
    public static partial void LogHandlingGetSimilarAuctions(this ILogger logger, Guid auctionId, int limit);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.GetSimilarAuctionsNoResults, Level = LogLevel.Warning, Message = "No similar auctions found for auction {AuctionId}.")]
    public static partial void LogNoSimilarAuctionsFound(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.GetSimilarAuctionsSuccess, Level = LogLevel.Information, Message = "Retrieved {Count} similar auctions for auction {AuctionId}.")]
    public static partial void LogSimilarAuctionsSuccess(this ILogger logger, Guid auctionId, int count);
}
