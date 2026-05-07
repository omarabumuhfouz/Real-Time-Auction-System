namespace MazadZone.Application.Features.Authentication.Commands.Logout;

internal static partial class LogoutLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.LogoutSuccess,
        Level = LogLevel.Information,
        Message = "User {UserId} logged out successfully for session: {HashedToken}")]
    public static partial void LogLogoutSuccess(ILogger logger, Guid userId, string hashedToken);

    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.LogoutFromAllDevices,
        Level = LogLevel.Information,
        Message = "User {UserId} performed a global logout (all devices invalidated).")]
    public static partial void LogLogoutAllDevices(ILogger logger, Guid userId);

    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.LogoutSessionNotFound,
        Level = LogLevel.Warning,
        Message = "Logout attempt for User {UserId} but the provided token was not found or already inactive: {HashedToken}")]
    public static partial void LogLogoutTokenNotFound(ILogger logger, Guid userId, string hashedToken);
}