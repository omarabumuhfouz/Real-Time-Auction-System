using MazadZone.Domain.Auctions;
using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Sellers.Commands.Verify;

internal static partial class VerifyLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Sellers.VerifySuccess,
        Level = LogLevel.Information,
        Message = "Seller {SellerId} has been successfully verified.")]
    public static partial void LogSuccess(ILogger logger, SellerId sellerId);
}