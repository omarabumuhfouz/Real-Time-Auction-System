using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.Confirm;

public sealed class NotifySellerOnOrderConfirmedDomainEventHandler 
    : INotificationHandler<OrderConfirmedDomainEvent>
{
    private readonly ISellerRepository _sellerRepository;
    private readonly ISender _sender;

    public NotifySellerOnOrderConfirmedDomainEventHandler(
        ISellerRepository sellerRepository,
        ISender sender)
    {
        _sellerRepository = sellerRepository;
        _sender = sender;
    }

    public async Task Handle(OrderConfirmedDomainEvent notification, CancellationToken ct)
    {
        var seller = await _sellerRepository.GetByAuctionIdAsync(notification.AuctionId, ct);
        if (seller is null) return;

        var title = "Payment Confirmed - Action Required!";
        var message = $@"Great news! The order for your auction (Order #{notification.OrderId.Value}) has been confirmed. 
                      Payment has been secured. You can now proceed to ship the item to the bidder. 
                      Please update the order status to 'Shipped' once you have the tracking information.";

        await _sender.Send(new CreateNotificationCommand(
            seller.Id,
            NotificationMethods.ReceiveNotification,
            title,
            message
        ));
    }
}