namespace MazadZone.Application.Features.Auctions.Commands.CreateAuction;

public static partial class CreateAuctionLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionAttempt, Level = LogLevel.Information, Message = "Handling CreateAuctionCommand .")]
    public static partial void LogHandlingCreateAuction(this ILogger logger);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionDomainViolation, Level = LogLevel.Warning, Message = "Failed to create auction. Reason: {Reason}")]
    public static partial void LogFailedToCreateAuction(this ILogger logger, string reason);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionSuccess, Level = LogLevel.Information, Message = "Auction created successfully with ID: {AuctionId}.")]
    public static partial void LogAuctionCreated(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionDomainViolation, Level = LogLevel.Error, Message = "Domain violation while creating auction. Error: {ErrorMessage}")]
    public static partial void LogDomainViolation(this ILogger logger, string errorMessage);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionJobScheduled, Level = LogLevel.Information,Message = "Auction {auctionId} will closed at {closeAt}.")]
    public static partial void LogJobScheduled(this ILogger logger, Guid auctionId, DateTime closeAt);

}