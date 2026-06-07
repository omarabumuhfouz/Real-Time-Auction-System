using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.Create;

/// <summary>
/// Notifies the winning bidder immediately after an order is generated from an auction win.
/// </summary>
public sealed class NotifyBidderOnOrderCreatedDomainEventHandler 
    : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly ISender _sender;

    public NotifyBidderOnOrderCreatedDomainEventHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken ct)
    {

           var title = "Auction Won! Action Required";
           var message = "Congratulations! You have the winning bid. Please confirm your shipping and payment details to finalize your order.";

        await _sender.Send(new CreateNotificationCommand(
                    notification.BidderId,
                    NotificationMethods.ReceiveNotification,
                    title,
                    message
                ));
    }
}