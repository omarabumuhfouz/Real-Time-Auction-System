using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Features.Orders.Commands.Deliver;
using MazadZone.Domain.Notifications;

namespace Tests.Application.Features.Orders.Commands.Deliver;

public class NotifyBidderOnOrderDeliveredDomainEventHandlerTests : OrderBaseTest<NotifyBidderOnOrderDeliveredDomainEventHandler>
{
    [Fact]
    public async Task Handle_OrderDelivered_SendsNotificationToBidder()
    {
        // Arrange
        var domainEvent = OrderHelper.CreateOrderDeliveredEvent();

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // Verify the correct command type, target BidderId, and notification delivery method
        await _sender.Received(1).Send(
            Arg.Is<CreateNotificationCommand>(c => 
                c.UserId == domainEvent.BidderId &&
                c.Method == NotificationMethods.ReceiveNotification
            ), 
            Arg.Any<CancellationToken>()
        );
    }
}