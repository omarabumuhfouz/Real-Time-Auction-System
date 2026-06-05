using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;
using MediatR;

namespace MazadZone.Application.Features.Orders.Commands.Deliver;

/// <summary>
/// Notifies the seller that the bidder has successfully received the item.
/// </summary>
public sealed class NotifySellerOnOrderDeliveredDomainEventHandler 
    : INotificationHandler<OrderDeliveredDomainEvent>
{
    private readonly ISellerRepository _sellerRepository;
    private readonly ISender _sender;

    public NotifySellerOnOrderDeliveredDomainEventHandler(
        ISellerRepository sellerRepository,
        ISender sender)
    {
        _sellerRepository = sellerRepository;
        _sender = sender;
    }

    public async Task Handle(OrderDeliveredDomainEvent notification, CancellationToken ct)
    {
        // 1. Get the Seller details
        var seller = await _sellerRepository.GetByAuctionIdAsync(notification.AuctionId, ct);
        if (seller is null) return;

        // 3. Create a professional and informative title
        const string title = "Product Received! Delivery Successful 📦";

        // 4. Draft the message regarding the completion of the sale
        var message = $@"Great news! The bidder has received your item (Order #{notification.OrderId.Value}). 
                        
The delivery is officially complete. The funds will be released to your account according to the platform's payout schedule. Keep an eye out for feedback from the bidder!";

        await _sender.Send(new CreateNotificationCommand(
                            seller.Id,
                            NotificationMethods.ReceiveNotification,
                            title,
                            message
                    ));

    }
}