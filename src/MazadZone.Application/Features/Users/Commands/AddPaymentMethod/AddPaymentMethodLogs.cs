using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.AddPaymentMethod;

internal static partial class AddPaymentMethodLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.PaymentMethods.AddAttempt,
        Level = LogLevel.Information,
        Message = "Attempting to add a payment method for User {UserId}.")]
    public static partial void LogAttempt(ILogger logger, UserId userId);

    [LoggerMessage(
        EventId = MazadLogEvents.PaymentMethods.AddDomainViolation,
        Level = LogLevel.Warning,
        Message = "Payment method creation rejected for User {UserId}: {ErrorCode}.")]
    public static partial void LogDomainViolation(ILogger logger, UserId userId, string errorCode);

    [LoggerMessage(
        EventId = MazadLogEvents.PaymentMethods.AddSuccess,
        Level = LogLevel.Information,
        Message = "Payment method {PaymentMethodId} added successfully for User {UserId}.")]
    public static partial void LogSuccess(ILogger logger, UserId userId, PaymentMethodId paymentMethodId);
}
