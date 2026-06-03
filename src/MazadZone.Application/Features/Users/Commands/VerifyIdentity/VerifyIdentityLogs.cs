using MazadZone.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Users.Commands.VerifyIdentity;

internal static partial class VerifyIdentityLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Users.VerifyIdentityAttempt,
        Level = LogLevel.Information,
        Message = "Identity verification process started for user {UserId}. Status transitioned to Pending.")]
    public static partial void LogAttempt(ILogger logger, UserId userId);

    [LoggerMessage(
        EventId = MazadLogEvents.Users.VerifyIdentitySuccess,
        Level = LogLevel.Information,
        Message = "Identity verification completed successfully for user {UserId}. Associated profiles verified.")]
    public static partial void LogSuccess(ILogger logger, UserId userId);

    [LoggerMessage(
        EventId = MazadLogEvents.Users.VerifyIdentityRejected,
        Level = LogLevel.Warning,
        Message = "Identity verification rejected for user {UserId}. Reason: {Reason}")]
    public static partial void LogRejected(ILogger logger, UserId userId, string reason);
}
