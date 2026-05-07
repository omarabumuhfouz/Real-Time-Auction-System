using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.ChangePassword;

public static partial class ChangePasswordLogs
{


    [LoggerMessage(
        Level = LogLevel.Critical,
        EventId = 111,
        Message = "Change Password Failed: {ErrorCode} - Internal error during password hashing for UserId {UserId}.")]
    public static partial void LogPasswordHashingError(this ILogger logger, string errorCode, UserId userId);

    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 112,
        Message = "Change Password Success: User {UserId} successfully updated their password.")]
    public static partial void LogPasswordChangedSuccessfully(this ILogger logger, UserId userId);
}