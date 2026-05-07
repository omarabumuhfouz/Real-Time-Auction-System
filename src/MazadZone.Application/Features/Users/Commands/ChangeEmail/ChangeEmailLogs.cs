using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Features.Users.ChangeEmail;

public static partial class ChangeEmailLogs
{
    // EventId 101: Conflict error
    [LoggerMessage(
        Level = LogLevel.Warning,
        EventId = 106,
        Message = "ChangeEmail Failed: {ErrorCode} - The email {Email} is already taken.")]
    public static partial void LogEmailConflict(this ILogger logger, string errorCode, string email);


    // EventId 105: Success Audit
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 107,
        Message = "ChangeEmail Success: User {UserId} changed their email to {NewEmail}.")]
    public static partial void LogEmailChangedSuccessfully(this ILogger logger, UserId userId, string newEmail);
}