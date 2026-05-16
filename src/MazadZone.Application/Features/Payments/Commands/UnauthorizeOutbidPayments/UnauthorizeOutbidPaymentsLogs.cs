namespace MazadZone.Application.Features.Payments.Commands.UnauthorizeOutbidPayments;

internal static partial class UnauthorizeOutbidPaymentsLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Payments.UnauthorizeAttempt,
        Level = LogLevel.Information,
        Message = "Attempting to unauthorize payment holds for {OutbidCount} outbid bidders")]
    public static partial void LogAttempt(ILogger logger, int outbidCount);

    [LoggerMessage(
        EventId = MazadLogEvents.Payments.UnauthorizeSuccess,
        Level = LogLevel.Information,
        Message = "Successfully released payment holds for {OutbidCount} outbid bidders")]
    public static partial void LogSuccess(ILogger logger, int outbidCount);

    [LoggerMessage(
        EventId = MazadLogEvents.Payments.UnauthorizeFailure,
        Level = LogLevel.Error,
        Message = "Failed to release payment holds for {OutbidCount} outbid bidders. Error: {ErrorMessage}")]
    public static partial void LogFailure(ILogger logger, int outbidCount, string errorMessage);
}
