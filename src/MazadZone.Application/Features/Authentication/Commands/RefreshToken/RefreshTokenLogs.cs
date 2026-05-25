namespace MazadZone.Application.Features.Authentication.Commands.RefreshToken;

using MazadZone.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

public static partial class RefreshTokenLogs
{
    // 1. User not found by token
    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.InvalidRefreshTokenProvided,
        Level = LogLevel.Warning,
        Message = "Token refresh failed. No user found associated with the provided refresh token hash.")]
    public static partial void LogInvalidRefreshTokenProvided(ILogger logger);

    // 2. Token found, but explicitly revoked or expired
    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.FailedRefreshTokenAttempt,
        Level = LogLevel.Warning,
        Message = "Failed refresh token attempt for User {UserId}. The token was not active or already revoked.")]
    public static partial void LogFailedRefreshTokenAttempt(ILogger logger, UserId userId);

    // 3. Domain logic rejected the rotation (e.g., max sessions reached, user banned)
    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.TokenRotationRejected,
        Level = LogLevel.Warning,
        Message = "Token rotation rejected by Domain for User {UserId}. Reason: {ErrorCode}")]
    public static partial void LogTokenRotationRejected(ILogger logger, UserId userId, string errorCode);

    // 4. Success!
    [LoggerMessage(
        EventId = MazadLogEvents.Authentication.TokenRefreshSuccess,
        Level = LogLevel.Information,
        Message = "Successfully refreshed access and refresh tokens for User {UserId}.")]
    public static partial void LogTokenRefreshSuccess(ILogger logger, UserId userId);
}