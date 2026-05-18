namespace MazadZone.Application.Features.Orders.Commands.ReplyToFeedback;
public record ReplyToFeedbackCommand(OrderId OrderId, string ReplyText) : ICommand<Unit>;