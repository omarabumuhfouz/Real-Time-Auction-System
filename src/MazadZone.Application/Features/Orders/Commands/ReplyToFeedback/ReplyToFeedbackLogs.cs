using MazadZone.Domain.Orders;
using Microsoft.Extensions.Logging;
// using MazadZone.Application.Common.Logging; <-- Add the namespace for your Orders class here

namespace MazadZone.Application.Features.Orders.Commands.ReplyToFeedback;

public static partial class ReplyToFeedbackLogs
{
    [LoggerMessage(
        EventId = MazadLogEvents.Orders.ReplyToFeedbackAttempt, 
        Level = LogLevel.Information, 
        Message = "Seller attempting to reply to feedback for OrderId: {OrderId}.")]
    public static partial void LogReplyAttempt(ILogger logger, OrderId orderId);

    [LoggerMessage(
        EventId = MazadLogEvents.Orders.ReplyToFeedbackDomainViolation, 
        Level = LogLevel.Warning, 
        Message = "Failed to add feedback reply for OrderId: {OrderId}. Reason: {ErrorMessage}.")]
    public static partial void LogReplyFailure(ILogger logger, OrderId orderId, string errorMessage);

    [LoggerMessage(
        EventId = MazadLogEvents.Orders.ReplyToFeedbackSuccess, 
        Level = LogLevel.Information, 
        Message = "Successfully added feedback reply for OrderId: {OrderId}.")]
    public static partial void LogReplySuccess(ILogger logger, OrderId orderId);
}