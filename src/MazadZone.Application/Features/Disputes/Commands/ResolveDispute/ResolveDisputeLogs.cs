namespace MazadZone.Application.Features.Orders.Commands.ResolveDispute;

internal static partial class ResolveDisputeLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Orders.ResolveDisputeAttempt, 
        Level = LogLevel.Information, 
        Message = "Attempting to resolve dispute for dispute with ID: {DisputeId}. Resolution: {Resolution}")]
    public static partial void LogAttempt(ILogger logger, DisputeId disputeId, string resolution);

    [LoggerMessage(
        EventId = MazadLogEvents.Orders.ResolveDisputeDomainViolation, 
        Level = LogLevel.Warning, 
        Message = "Domain logic prevented resolving dispute Code {Code}. Reason: {Error}")]
    public static partial void LogDomainViolation(ILogger logger, string code, string error);

    [LoggerMessage(
        EventId = MazadLogEvents.Orders.ResolveDisputeSuccess, 
        Level = LogLevel.Information, 
        Message = "Dispute successfully resolved for Dispute {DisputeId} and persisted.")]
    public static partial void LogSuccess(ILogger logger,  DisputeId disputeId);
}