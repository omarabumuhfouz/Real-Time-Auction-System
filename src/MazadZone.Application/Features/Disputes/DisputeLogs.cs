namespace MazadZone.Application.Features.Disputes;

public static partial class DisputeLogs
{

    [LoggerMessage(
        EventId = MazadLogEvents.Global.NotFound,
        Level = LogLevel.Warning,
        Message = "Resource missing: Dispute with ID {disputeId} could not be found.")]
    public static partial void LogDisputeNotFound(ILogger logger, DisputeId disputeId);

    [LoggerMessage(
        EventId = MazadLogEvents.Global.NotFound,
        Level = LogLevel.Warning,
        Message = "Resource missing: Dispute Related to Order with ID {orderId} could not be found.")]
    public static partial void LogDisputeNotFound(ILogger logger, OrderId orderId);

}