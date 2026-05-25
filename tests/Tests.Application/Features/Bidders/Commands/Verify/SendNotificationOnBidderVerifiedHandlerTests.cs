using MazadZone.Application.Features.Bidders.Commands.Verify;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Bidders.Events;

namespace Tests.Application.Features.Bidders.EventHandlers;

public class SendNotificationOnBidderVerifiedHandlerTests : BidderBaseTest<SendNotificationOnBidderVerifiedHandler>
{
    [Fact]
    public async Task Handle_BidderVerifiedEventIsRaised_SendsInAppNotification()
    {
        // Arrange
        var domainEvent = new BidderVerifiedDomainEvent(UserId.New());

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