using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Commands.Verify;

internal static partial class VerifySellerLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Sellers.VerifySuccess,
        Level = LogLevel.Information,
        Message = "Seller {SellerId} has been successfully verified.")]
    public static partial void LogSuccess(ILogger logger, SellerId sellerId);
}