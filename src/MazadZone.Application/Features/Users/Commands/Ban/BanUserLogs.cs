using MazadZone.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Users.Commands.Ban;

public static partial class BanUserLogs
{
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 113,
        Message = "User {UserId} has been Banned. Reason: {Reason}")]
    public static partial void LogUserBanned(this ILogger logger, UserId userId, string reason);

[LoggerMessage(
        Level = LogLevel.Warning,
        EventId = 114,
        Message = "Ban Rejected: User {UserId} state transition failed with error {ErrorCode}.")]
    public static partial void LogBanDomainError(this ILogger logger, UserId userId, string errorCode);
}