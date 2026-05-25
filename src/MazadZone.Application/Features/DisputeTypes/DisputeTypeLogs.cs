namespace MazadZone.Features.DisputeTypes;

using Microsoft.Extensions.Logging;

public static partial class DisputeTypeLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.DisputeTypes.DomainViolation,
        Level = LogLevel.Warning,
        Message = "Domain rule violation occurred. Error Code: {ErrorCode} | Details: {ErrorMessage}")]
    public static partial void LogDomainViolation(ILogger logger, string errorCode, string errorMessage);


    [LoggerMessage(EventId = MazadLogEvents.Global.NotFound, Level = LogLevel.Warning, Message = "Dispute Type with ID '{Id}' not found for deletion.")]
    public static partial void LogNotFound(ILogger logger, DisputeTypeId id);

[LoggerMessage(
            EventId = MazadLogEvents.DisputeTypes.CreateSuccess, 
        Level = LogLevel.Information, 
        Message = "Successfully created Dispute Type with ID: {Id}")]
    public static partial void LogCreateSuccess(ILogger logger, DisputeTypeId id);

    [LoggerMessage(
        EventId = MazadLogEvents.DisputeTypes.DeleteSuccess,
        Level = LogLevel.Information,
        Message = "Successfully deleted Dispute Type with ID: {Id}")]
    public static partial void LogDeleteSuccess(ILogger logger, DisputeTypeId id);

    [LoggerMessage(
            EventId = MazadLogEvents.DisputeTypes.RestoreSuccess,
            Level = LogLevel.Information,
            Message = "Successfully restored Dispute Type with ID: {Id}")]
    public static partial void LogRestoreSuccess(ILogger logger, DisputeTypeId id);

[LoggerMessage(
        EventId = MazadLogEvents.DisputeTypes.UpdateSuccess, 
        Level = LogLevel.Information, 
        Message = "Successfully updated Dispute Type with ID: {Id}")]
    public static partial void LogUpdateSuccess(ILogger logger, DisputeTypeId id);

}