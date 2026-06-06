using MazadZone.Domain.Notifications;

namespace MazadZone.Application.Features.Notifications.Commands.CreateNotification;

public record CreateNotificationCommand(
    UserId UserId,
    string Method,
    string Title,
    string Message) : ICommand<NotificationId>;