using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Notifications.Commands.CreateNotification;

public record CreateNotificationCommand(
    UserId UserId,
    string Method,
    string Title,
    string Message) : ICommand<NotificationId>;