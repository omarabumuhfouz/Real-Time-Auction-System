using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.AddFeedback;
public sealed class NotifySellerOnFeedbackLeftDomainEventHandler : INotificationHandler<FeedbackLeftDomainEvent>
{
    private readonly ISellerRepository _sellerRepository;
    private readonly ISender _sender;

    public NotifySellerOnFeedbackLeftDomainEventHandler(ISellerRepository sellerRepository, ISender sender)
    {
        _sellerRepository = sellerRepository;
        _sender = sender;
    }

    public async Task Handle(FeedbackLeftDomainEvent notification, CancellationToken ct)
    {
        var seller = await _sellerRepository.GetByAuctionIdAsync(notification.AuctionId, ct);
        if (seller is null) return;

        var title = "New Feedback Received";
        var message = $"You received a {notification.Rating}-star review!";

        await _sender.Send(new CreateNotificationCommand(
                                            seller.Id,
                                            NotificationMethods.ReceiveNotification,
                                            title,
                                            message
                                    ));
    }
}