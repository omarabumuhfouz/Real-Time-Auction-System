using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.Suspend;

public static partial class SuspendUserLogs
{
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 113,
        Message = "User {UserId} suspended until {UntilDate}.")]
    public static partial void LogUserSuspended(this ILogger logger, UserId userId, DateTime untilDate);

[LoggerMessage(
        Level = LogLevel.Warning,
        EventId = 114,
        Message = "Suspension Rejected: User {UserId} state transition failed (Error: {ErrorCode}).")]
    public static partial void LogSuspensionDomainError(this ILogger logger, UserId userId, string errorCode);
}