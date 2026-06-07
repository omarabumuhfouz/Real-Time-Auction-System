using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Features.Orders.Commands.Ship;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Orders.Events;

namespace Tests.Application.Features.Orders.Commands.Ship;

public class NotifyBidderOnOrderShippedDomainEventHandlerTests 
    : OrderBaseTest<NotifyBidderOnOrderShippedDomainEventHandler>
{
    [Fact]
    public async Task Handle_OrderShipped_SendsNotificationToBidder()
    {
        // 1. Arrange
        var domainEvent = new OrderShippedDomainEvent(OrderId.New(), UserId.New());

        // 2. Act
        await Handler.Handle(domainEvent, default);

        // 3. Assert - Listen to _sender and target the correct command contract
        await _sender.Received(1).Send(
            Arg.Is<CreateNotificationCommand>(c => 
                c.UserId == domainEvent.BidderId &&
                c.Method == NotificationMethods.ReceiveNotification
            ),
            Arg.Any<CancellationToken>());
    }
}