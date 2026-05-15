using MazadZone.Domain.Payments.ValueObjects;
using MzadZone.Domain.Payments;

namespace MazadZone.Application.Features.Payments.Commands.CaptureAuthorizedHold;

internal static partial class CaptureAuthorizedHoldLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Payments.CaptureHoldAttempt,
        Level = LogLevel.Information,
        Message = "Attempting to capture authorized hold for Payment {PaymentId} with Gateway Hold ID: {GatewayAuthHoldId}")]
    public static partial void LogAttempt(ILogger logger, PaymentId paymentId, string gatewayAuthHoldId);

    [LoggerMessage(
        EventId = MazadLogEvents.Payments.CaptureHoldSuccess,
        Level = LogLevel.Information,
        Message = "Successfully captured authorized hold for Payment {PaymentId} with Transaction ID: {TransactionId}")]
    public static partial void LogSuccess(ILogger logger, PaymentId paymentId, string transactionId);

    [LoggerMessage(
        EventId = MazadLogEvents.Payments.CaptureHoldFailure,
        Level = LogLevel.Error,
        Message = "Failed to capture authorized hold for Payment {PaymentId}. Error: {ErrorMessage}")]
    public static partial void LogFailure(ILogger logger, PaymentId paymentId, string errorMessage);

    [LoggerMessage(
        EventId = MazadLogEvents.Payments.CaptureHoldGatewayFailure,
        Level = LogLevel.Error,
        Message = "Payment gateway failed to capture hold for Payment {PaymentId} with Gateway Hold ID: {GatewayAuthHoldId}")]
    public static partial void LogCaptureFailed(ILogger logger, PaymentId paymentId, string gatewayAuthHoldId);
}
