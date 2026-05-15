using MazadZone.Domain.Entities.Orders;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.ResolveDispute;

public sealed class NotifySellerOnDisputeResolvedDomainEventHandler 
    : INotificationHandler<DisputeResolvedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISellerRepository _sellerRepository;
    private readonly INotificationRepository _notificationRepository;

    public NotifySellerOnDisputeResolvedDomainEventHandler(
        IOrderRepository orderRepository,
        ISellerRepository sellerRepository,
        INotificationRepository notificationRepository)
    {
        _orderRepository = orderRepository;
        _sellerRepository = sellerRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(DisputeResolvedDomainEvent notification, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(notification.OrderId.Value, ct);
        if (order is null) return;

        var seller = await _sellerRepository.GetByAuctionIdAsync(order.AuctionId, ct);
        if (seller is null) return;

        const string title = "Dispute Resolution Reached: Order #{0}";
        var message = $@"The dispute regarding your auction has been resolved by our team.

Admin Resolution: {notification.Resolution}

Please check your dashboard for any updates regarding your payout or order status.";

        await _notificationRepository.NotifySellerAsync(
            seller.Id.Value, // Assuming UserId is used for notifications
            string.Format(title, order.Id.Value),
            message,
            ct);
    }
}