namespace MazadZone.Application.Features.Auctions.Commands.CreateAuction;

public static partial class CreateAuctionLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionAttempt, Level = LogLevel.Information, Message = "Handling CreateAuctionCommand for ItemId: {ItemId}.")]
    public static partial void LogHandlingCreateAuction(this ILogger logger, string itemId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionDomainViolation, Level = LogLevel.Warning, Message = "Failed to create auction with title {ItemId}. Reason: {Reason}")]
    public static partial void LogFailedToCreateAuction(this ILogger logger, string itemId, string reason);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionSuccess, Level = LogLevel.Information, Message = "Auction with ItemId {ItemId} created successfully with ID: {AuctionId}.")]
    public static partial void LogAuctionCreated(this ILogger logger, string itemId, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CreateAuctionDomainViolation, Level = LogLevel.Error, Message = "Domain violation while creating auction with ItemId {ItemId}. Error: {ErrorMessage}")]
    public static partial void LogDomainViolation(this ILogger logger, string itemId, string errorMessage);

}