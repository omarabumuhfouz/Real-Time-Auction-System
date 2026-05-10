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
    private readonly INotificationRepository _notificationService;
    private readonly ISellerRepository _sellerRepository;

    public NotifySellerOnOrderDeliveredDomainEventHandler(
        INotificationRepository notificationService,
        ISellerRepository sellerRepository)
    {
        _notificationService = notificationService;
        _sellerRepository = sellerRepository;
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

        // 5. Send notification to the Seller
        await _notificationService.NotifySellerAsync(
            seller.Id.Value, 
            title, 
            message);
    }
}