namespace MazadZone.Application.Features.Auctions.Commands.CancelAuction;
public static partial class CancelAuctionLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.CancelAuctionAttempt, Level = LogLevel.Information, Message = "Handling CancelAuctionCommand for AuctionId: {AuctionId}.")]
    public static partial void LogHandlingCancelAuction(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CancelAuctionNotFound, Level = LogLevel.Warning, Message = "Auction with ID {AuctionId} not found for cancellation.")]
    public static partial void LogAuctionNotFound(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CancelAuctionSuccess, Level = LogLevel.Information, Message = "Auction with ID {AuctionId} cancelled successfully.")]
    public static partial void LogAuctionCancelled(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CancelAuctionDomainViolation, Level = LogLevel.Error, Message = "Domain violation for AuctionId: {AuctionId}. Error: {ErrorMessage}")]
    public static partial void LogDomainViolation(this ILogger logger, Guid auctionId, string errorMessage);
}