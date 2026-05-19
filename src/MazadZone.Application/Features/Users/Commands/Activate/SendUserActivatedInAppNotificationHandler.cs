using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.Activate;

public class SendUserActivatedInAppNotificationHandler : INotificationHandler<UserActivatedDomainEvent>
{
    private readonly INotificationRepository _notificationRepo;

    public SendUserActivatedInAppNotificationHandler(INotificationRepository notificationRepo)
    {
        _notificationRepo = notificationRepo;
    }

    public async Task Handle(UserActivatedDomainEvent notification, CancellationToken ct)
    {
        const string title = "Welcome Back! Account Activated";
        const string message = "Your account suspension has been lifted. You can now participate in auctions again.";

        await _notificationRepo.NotifyUserAsync(notification.UserId, title, message, ct);
    }
}