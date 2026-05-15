using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users;
using MazadZone.Domain.ValueObjects;

namespace MazadZone.Application.Features.Auctions.Commands.PlaceBid;

internal static partial class PlaceBidLog
{
    [LoggerMessage(
        EventId = MazadLogEvents.Auctions.PlaceBidAttempt,
        Level = LogLevel.Information,
        Message = "Attempting to place bid on Auction {AuctionId} by Bidder {BidderId} for amount {Amount}.")]
    public static partial void LogPlaceBidAttempt(ILogger logger, Guid auctionId, Guid bidderId, decimal amount);

    [LoggerMessage(
        EventId = MazadLogEvents.Auctions.PlaceBidAuctionNotFound,
        Level = LogLevel.Warning,
        Message = "Auction with ID {AuctionId} not found during bid placement.")]
    public static partial void LogAuctionNotFound(ILogger logger, Guid auctionId);

    [LoggerMessage(
        EventId = MazadLogEvents.Auctions.PlaceBidDomainViolation,
        Level = LogLevel.Warning,
        Message = "Unable to place bid on Auction {AuctionId}. Reason: {Reason}")]
    public static partial void LogDomainViolation(ILogger logger, Guid auctionId, string reason);

    [LoggerMessage(
        EventId = MazadLogEvents.Auctions.PlaceBidPaymentAuthorizationFailed,
        Level = LogLevel.Error,
        Message = "Payment authorization failed for Auction {AuctionId} by Bidder {BidderId}.")]
    public static partial void LogPaymentAuthorizationFailed(ILogger logger, Guid auctionId, Guid bidderId);

    [LoggerMessage(
        EventId = MazadLogEvents.Auctions.PlaceBidPersistenceFailed,
        Level = LogLevel.Error,
        Message = "Failed to persist bid for Auction {AuctionId} by Bidder {BidderId}. Error: {ErrorMessage}")]
    public static partial void LogPersistenceFailed(ILogger logger, Guid auctionId, Guid bidderId, string errorMessage);

    [LoggerMessage(
        EventId = MazadLogEvents.Auctions.PlaceBidSuccess,
        Level = LogLevel.Information,
        Message = "Bid placed successfully for Auction {AuctionId} by Bidder {BidderId}.")]
    public static partial void LogSuccess(ILogger logger, Guid auctionId, Guid bidderId);
 
}