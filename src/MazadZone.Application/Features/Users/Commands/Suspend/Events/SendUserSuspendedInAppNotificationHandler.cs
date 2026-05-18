using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.Suspend.Events;

public class SendUserSuspendedInAppNotificationHandler(
    INotificationRepository notificationRepo) : INotificationHandler<UserSuspendedDomainEvent>
{
    private readonly INotificationRepository _notificationRepo = notificationRepo;

    public async Task Handle(UserSuspendedDomainEvent notification, CancellationToken ct)
    {
        var title = "Account Suspended";
        var message = $"Your account has been suspended due to: {notification.Reason}. " +
                      $"Estimated return: {notification.ReinstatementDate?.ToShortDateString() ?? "Indefinite"}";

        await _notificationRepo.NotifyUserAsync(notification.UserId, title, message, ct);
    }
}