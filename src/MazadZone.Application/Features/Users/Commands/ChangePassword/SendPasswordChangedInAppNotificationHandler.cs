using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.ChangePassword;

public class SendPasswordChangedInAppNotificationHandler(INotificationRepository notificationRepo) : INotificationHandler<UserPasswordChangedDomainEvent>
{
    private readonly INotificationRepository _notificationRepo = notificationRepo;

    public async Task Handle(UserPasswordChangedDomainEvent notification, CancellationToken ct)
    {
        const string title = "Security Update: Password Changed";

        await _notificationRepo.NotifyUserAsync(
            notification.UserId,
            title,
            "Your account password was successfully updated.",
            ct);
    }
}