using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Sellers.Commands.BecomeSeller;

internal static partial class BecomeSellerLogs
{
        [LoggerMessage(
        EventId = MazadLogEvents.Sellers.BecomeSellerDomainViolation,
        Level = LogLevel.Warning,
        Message = "Domain rule violation for Bidder {BidderId} becoming seller: {ErrorCode}")]
    public static partial void LogDomainRuleViolation(ILogger logger, BidderId bidderId, string errorCode);


    [LoggerMessage(
        EventId = MazadLogEvents.Sellers.BecomeSellerSuccess,
        Level = LogLevel.Information,
        Message = "Bidder {BidderId} has successfully become a Seller.")]
    public static partial void LogSuccess(ILogger logger, BidderId bidderId);
}