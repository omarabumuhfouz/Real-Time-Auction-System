using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Features.Orders.Events.NotifyBidderOnFeedbackReplied;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Orders.Events;

namespace Tests.Application.Features.Orders.Events.NotifyBidderOnFeedbackReplied;

public class NotifyBidderOnFeedbackRepliedEventHandlerTests : OrderBaseTest<NotifyBidderOnFeedbackRepliedEventHandler>
{
    [Fact]
    public async Task Handle_FeedbackReplied_SendsNotificationToBidder()
    {
       var domainEvent = new FeedbackRepliedDomainEvent(OrderId.New(), UserId.New());
    
    // Exact expected command (Assuming it's a record/supports value equality)
    var expectedCommand = new CreateNotificationCommand(
        domainEvent.BidderId,
        NotificationMethods.ReceiveNotification,
        "Seller Replied to Your Feedback",
        $"The seller left a response to your feedback on Order #{domainEvent.OrderId.Value}."
    );

    // Act
    await Handler.Handle(domainEvent, default);

    // Assert
    await _sender.Received(1).Send(expectedCommand, Arg.Any<CancellationToken>());
    }
}