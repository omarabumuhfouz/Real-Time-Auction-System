using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Commands.UpdateBankDetails;

internal static partial class UpdateBankDetailsLogs
{
[LoggerMessage(
        EventId = MazadLogEvents.Sellers.BankUpdateDomainViolation,
        Level = LogLevel.Warning,
        Message = "Domain rule violation for Seller {SellerId} during bank update: {ErrorCode}")]
    public static partial void LogDomainRuleViolation(ILogger logger, SellerId sellerId, string errorCode);

    [LoggerMessage(
        EventId = MazadLogEvents.Sellers.BankUpdateSuccess,
        Level = LogLevel.Information,
        Message = "Bank details successfully updated for Seller {SellerId}. Verification reset.")]
    public static partial void LogSuccess(ILogger logger, SellerId sellerId);
}