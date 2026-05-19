using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.ChangeEmail.Events;

public class SendEmailChangedInAppNotificationHandler : INotificationHandler<UserEmailChangedDomainEvent>
{
    private readonly INotificationRepository _notificationRepo;

    public SendEmailChangedInAppNotificationHandler(INotificationRepository notificationRepo)
    {
        _notificationRepo = notificationRepo;
    }

    public async Task Handle(UserEmailChangedDomainEvent notification, CancellationToken ct)
    {
        const string title = "Security Alert: Email Updated";

        await _notificationRepo.NotifyUserAsync(
            notification.UserId,
            title,
            $"The email associated with your account was changed from {notification.OldEmail} to {notification.NewEmail}.",
            ct);
    }
}