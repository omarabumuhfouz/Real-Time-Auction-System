using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Common.Extensions;

public static partial class UserLoggingExtensions
{
    [LoggerMessage(
        Level = LogLevel.Error,
        EventId = 101,
        Message = "Search Failed: {ErrorCode} - User {UserId} does not exist.")]
    public static partial void LogUserNotFound(this ILogger logger, string errorCode, UserId userId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        EventId = 102,
        Message = "Search Failed: {ErrorCode} - No user found with email: {Email}")]
    public static partial void LogUserNotFound(this ILogger logger, string errorCode, string email);

    [LoggerMessage(
        Level = LogLevel.Warning,
        EventId = 103,
        Message = "Change Password Failed: {ErrorCode} - Invalid current password provided for UserId {UserId}.")]
    public static partial void LogInvalidCurrentPassword(this ILogger logger, string errorCode, UserId userId);

}