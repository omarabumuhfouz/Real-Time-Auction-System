namespace MazadZone.Application.Features.Authentication.Commands.RefreshToken;

public static partial class RefreshTokenLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.InvalidRefreshTokenProvided, 
        Level = LogLevel.Warning, 
        Message = "Token refresh failed. No user found associated with the provided refresh token hash: {HashedToken}")]
    public static partial void LogInvalidRefreshTokenProvided(this ILogger logger, string hashedToken);

    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.TokenRotationRejected, 
        Level = LogLevel.Warning, 
        Message = "Token rotation rejected by Domain for User {UserId}. Reason: {ErrorCode}")]
    public static partial void LogTokenRotationRejected(this ILogger logger, Guid userId, string errorCode);
}