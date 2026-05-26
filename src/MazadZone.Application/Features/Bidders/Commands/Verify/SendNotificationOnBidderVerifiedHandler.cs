using MazadZone.Domain.Bidders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Bidders.Commands.Verify;

public class SendNotificationOnBidderVerifiedHandler : INotificationHandler<BidderVerifiedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public SendNotificationOnBidderVerifiedHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(BidderVerifiedDomainEvent notification, CancellationToken ct)
    {
        var message = "Congratulations! Your account has been verified. You are ready to bid.";
        var title = "Account Verified";

        await _notificationRepository.NotifyBidderAsync(notification.BidderId.Value, title, message, ct);
    }
}