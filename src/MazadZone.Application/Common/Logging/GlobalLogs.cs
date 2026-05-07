using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;

public static partial class GlobalLogsh
{

    [LoggerMessage(
        EventId = MazadLogEvents.Global.NotFound,
        Level = LogLevel.Warning,
        Message = "Resource missing: Bidder with ID {BidderId} could not be found.")]
    public static partial void LogBidderNotFound(ILogger logger, BidderId bidderId);

    [LoggerMessage(
        EventId = MazadLogEvents.Global.NotFound,
        Level = LogLevel.Warning,
        Message = "Resource missing: Seller with ID {SellerId} could not be found.")]
    public static partial void LogSellerNotFound(ILogger logger, SellerId sellerId);

    [LoggerMessage(
        EventId = MazadLogEvents.Global.NotFound,
        Level = LogLevel.Warning,
        Message = "Resource missing: User with ID {userId} could not be found.")]
    public static partial void LogUserNotFound(ILogger logger, UserId userId);
}