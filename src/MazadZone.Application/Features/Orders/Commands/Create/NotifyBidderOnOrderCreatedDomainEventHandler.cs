using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.Create;

/// <summary>
/// Notifies the winning bidder immediately after an order is generated from an auction win.
/// </summary>
public sealed class NotifyBidderOnOrderCreatedDomainEventHandler 
    : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly INotificationRepository _notificationService;

    public NotifyBidderOnOrderCreatedDomainEventHandler(
        INotificationRepository notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken ct)
    {
        await _notificationService.NotifyBidderAsync(
            notification.BidderId.Value,
           title: "Auction Won! Action Required",
            message: "Congratulations! You have the winning bid. Please confirm your shipping and payment details to finalize your order.");
    }
}