using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Auctions.Queries.GetAuctionById;

public static partial class GetAuctionByIdLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.GetAuctionByIdAttempt, Level = LogLevel.Information, Message = "Handling GetAuctionByIdQuery for AuctionId: {AuctionId}.")]
    public static partial void LogHandlingGetAuctionById(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.GetAuctionByIdNotFound, Level = LogLevel.Warning, Message = "Auction with ID {AuctionId} not found.")]
    public static partial void LogAuctionNotFound(this ILogger logger, Guid auctionId);
}