namespace MazadZone.Application.Features.Bidders.Commands.Register;

public static partial class RegisterBidderLogs
{
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = MazadLogEvents.Bidders.Registered,
        Message = "Registration completed successfully. UserId: {UserId}, Email: {Email}.")]
    public static partial void LogRegistrationSuccess(this ILogger logger, Guid userId, string email);

    [LoggerMessage(
        Level = LogLevel.Error,
        EventId = MazadLogEvents.Bidders.ProfileError,
        Message = "Failed to create bidder profile for UserId: {UserId}. Error: {Error}")]
    public static partial void LogBidderProfileError(this ILogger logger, Guid userId, string error);

    [LoggerMessage(
        Level = LogLevel.Error,
        EventId = MazadLogEvents.Bidders.IdentityVerificationFailed,
        Message = "Identity verification failed for user {UserId}. Error: {ErrorMessage}.")]
    public static partial void LogIdentityVerificationFailed(this ILogger logger, Guid userId, string errorMessage);


    [LoggerMessage(
        Level = LogLevel.Warning,
        EventId = MazadLogEvents.Bidders.EmailConflict,
        Message = "Email conflict during registration. Email: {Email}. Error: {ErrorCode}.")]
    public static partial void LogEmailConflict(this ILogger logger, string errorCode, string email);
}