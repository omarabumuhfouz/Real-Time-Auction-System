using MazadZone.Domain.Payments.ValueObjects;
using MzadZone.Domain.Payments;

namespace MazadZone.Application.Features.Payments.Commands.RecordAuthorizationHold;

internal static partial class RecordAuthorizationHoldLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Payments.RecordAttempt,
        Level = LogLevel.Information,
        Message = "Attempting to record authorization hold for Payment {PaymentId} with Gateway Hold ID: {GatewayAuthHoldId}")]
    public static partial void LogAttempt(ILogger logger, PaymentId paymentId, string gatewayAuthHoldId);

    [LoggerMessage(
        EventId = MazadLogEvents.Payments.RecordSuccess,
        Level = LogLevel.Information,
        Message = "Successfully recorded authorization hold for Payment {PaymentId} with Gateway Hold ID: {GatewayAuthHoldId}")]
    public static partial void LogSuccess(ILogger logger, PaymentId paymentId, string gatewayAuthHoldId);

    [LoggerMessage(
        EventId = MazadLogEvents.Payments.RecordFailure,
        Level = LogLevel.Error,
        Message = "Failed to record authorization hold for Payment {PaymentId} with Gateway Hold ID: {GatewayAuthHoldId}. Error: {ErrorMessage}")]
    public static partial void LogFailure(ILogger logger, PaymentId paymentId, string gatewayAuthHoldId, string errorMessage);
}
