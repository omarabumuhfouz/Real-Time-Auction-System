using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;

internal static partial class BidderLoggingExtension
{
    // EventId 1001: Profile Not Found (Warning)
    [LoggerMessage(
        EventId = 150,
        Level = LogLevel.Warning,
        Message = "Bidder profile not found for BidderId: {BidderId}.")]
    public static partial void LogBidderNotFound(this ILogger logger, BidderId bidderId);

}