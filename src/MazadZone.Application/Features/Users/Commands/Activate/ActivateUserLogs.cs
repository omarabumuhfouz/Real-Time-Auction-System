using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.Activate;

public static partial class ActivateUserLogs
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        EventId = 111,
        Message = "Activation Rejected: User {UserId} is in a state that cannot be activated (Error: {ErrorCode}).")]
    public static partial void LogActivationDomainError(this ILogger logger, UserId userId, string errorCode);


    [LoggerMessage(Level = LogLevel.Information, EventId = 112, Message = "User {UserId} activated successfully.")]
    public static partial void LogUserActivated(this ILogger logger, UserId userId);

}