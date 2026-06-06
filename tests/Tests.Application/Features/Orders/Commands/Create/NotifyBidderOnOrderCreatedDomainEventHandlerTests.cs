using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Features.Orders.Commands.Create;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Orders.Events;

namespace Tests.Application.Features.Orders.Events;

public class NotifyBidderOnOrderCreatedDomainEventHandlerTests : OrderBaseTest<NotifyBidderOnOrderCreatedDomainEventHandler>
{
    [Fact]
    public async Task Handle_OrderCreated_SendsNotificationToBidder()
    {
        // Arrange
        var domainEvent = new OrderCreatedDomainEvent(
            OrderId.New(),
            UserId.New());

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Verify the correct command type and its internal data
        await _sender.Received(1).Send(
            Arg.Is<CreateNotificationCommand>(c => 
                c.UserId == domainEvent.BidderId &&
                c.Method == NotificationMethods.ReceiveNotification
            ), 
            Arg.Any<CancellationToken>()
        );
    }
}