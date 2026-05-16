namespace MazadZone.Application.Features.Auctions.Commands.EndAuction;

public static partial class EndAuctionLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.EndAuctionAttempt, Level = LogLevel.Information, Message = "Handling EndAuctionCommand for AuctionId: {AuctionId}.")]
    public static partial void LogHandlingEndAuction(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.EndAuctionNotFound, Level = LogLevel.Warning, Message = "Auction with ID {AuctionId} not found for ending.")]
    public static partial void LogAuctionNotFound(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.EndAuctionSuccess, Level = LogLevel.Information, Message = "Auction with ID {AuctionId} ended successfully.")]
    public static partial void LogAuctionEnded(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.EndAuctionDomainViolation, Level = LogLevel.Error, Message = "Domain violation for AuctionId: {AuctionId}. Error: {ErrorMessage}")]
    public static partial void LogDomainViolation(this ILogger logger, Guid auctionId, string errorMessage);
}