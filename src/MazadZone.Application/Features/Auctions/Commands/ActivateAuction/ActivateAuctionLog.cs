namespace MazadZone.Application.Features.Auctions.Commands.ActivateAuction;

public static partial class ActivateAuctionLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.ActivateAuctionAttempt, Level = LogLevel.Information, Message = "Handling ActivateAuctionCommand for AuctionId: {AuctionId}.")]
    public static partial void LogHandlingActivateAuction(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.ActivateAuctionNotFound, Level = LogLevel.Warning, Message = "Auction with ID {AuctionId} not found for activation.")]
    public static partial void LogAuctionNotFound(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.ActivateAuctionSuccess, Level = LogLevel.Information, Message = "Auction with ID {AuctionId} activated successfully.")]
    public static partial void LogAuctionActivated(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.ActivateAuctionDomainViolation, Level = LogLevel.Error, Message = "Domain violation for AuctionId: {AuctionId}. Error: {ErrorMessage}")]
    public static partial void LogDomainViolation(this ILogger logger, Guid auctionId, string errorMessage);
}