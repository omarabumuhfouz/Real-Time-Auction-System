using MazadZone.Domain.Sellers;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Sellers.Commands.Verify;

internal static partial class VerifySellerLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Sellers.VerifySuccess,
        Level = LogLevel.Information,
        Message = "Seller {SellerId} has been successfully verified.")]
    public static partial void LogSuccess(ILogger logger, UserId sellerId);
}