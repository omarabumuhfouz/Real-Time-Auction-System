namespace MazadZone.Application.Features.Auctions.Commands.CancelAuctionByAdmin;


public static partial class CancelAuctionByAdminLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.CancelAuctionByAdminAttempt, Level = LogLevel.Information, Message = "Handling CancelAuctionByAdminCommand for AuctionId: {AuctionId}.")]
    public static partial void LogHandlingCancelAuctionByAdmin(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CancelAuctionByAdminNotFound, Level = LogLevel.Warning, Message = "Auction with ID {AuctionId} not found for cancellation by admin.")]
    public static partial void LogAuctionNotFound(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CancelAuctionByAdminSuccess, Level = LogLevel.Information, Message = "Auction with ID {AuctionId} cancelled successfully by admin.")]
    public static partial void LogAuctionCancelled(this ILogger logger, Guid auctionId);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.CancelAuctionByAdminDomainViolation, Level = LogLevel.Error, Message = "Domain violation for AuctionId: {AuctionId}. Error: {ErrorMessage}")]
    public static partial void LogDomainViolation(this ILogger logger, Guid auctionId, string errorMessage);
}