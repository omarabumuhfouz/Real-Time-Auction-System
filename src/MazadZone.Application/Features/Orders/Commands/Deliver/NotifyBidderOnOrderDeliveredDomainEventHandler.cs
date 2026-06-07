using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.Deliver; 

/// <summary>
/// Notifies the bidder that their item has arrived and prompts them to leave a review for the seller.
/// </summary>
public sealed class NotifyBidderOnOrderDeliveredDomainEventHandler 
    : INotificationHandler<OrderDeliveredDomainEvent>
{
    private readonly ISender _sender;

    public NotifyBidderOnOrderDeliveredDomainEventHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task Handle(OrderDeliveredDomainEvent notification, CancellationToken ct)
    {
        const string title = "Item Delivered! How was your experience? ⭐";

        var message = $@"Great news! Your item from Order #{notification.OrderId.Value} has been delivered. 
        Please take a moment to leave feedback for the seller. Your reviews help keep the MazadZone community safe and reliable!";

        await _sender.Send(new CreateNotificationCommand(
                            notification.BidderId,
                            NotificationMethods.ReceiveNotification,
                            title,
                            message
                        ));
    }
}