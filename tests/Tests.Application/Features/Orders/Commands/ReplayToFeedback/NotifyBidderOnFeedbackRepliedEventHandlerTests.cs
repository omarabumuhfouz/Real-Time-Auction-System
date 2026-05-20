using MazadZone.Application.Features.Orders.Events.NotifyBidderOnFeedbackReplied;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Orders.Events;

namespace Tests.Application.Features.Orders.Events.NotifyBidderOnFeedbackReplied;

public class NotifyBidderOnFeedbackRepliedEventHandlerTests : OrderBaseTest<NotifyBidderOnFeedbackRepliedEventHandler>
{
    [Fact]
    public async Task Handle_FeedbackReplied_SendsNotificationToBidder()
    {
        // Arrange
        var domainEvent = new FeedbackRepliedDomainEvent(OrderId.New(), BidderId.New());

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        await _notificationRepository.Received(1).NotifyBidderAsync(
            domainEvent.BidderId.Value,
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}