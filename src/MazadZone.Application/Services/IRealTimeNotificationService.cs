using MazadZone.Domain.Notifications;

namespace MazadZone.Application.Services;

public interface IRealTimeNotificationService
{
    Task SendNotificationAsync(Guid userId, NotificationId notificationId, CancellationToken cancellationToken = default);
}
