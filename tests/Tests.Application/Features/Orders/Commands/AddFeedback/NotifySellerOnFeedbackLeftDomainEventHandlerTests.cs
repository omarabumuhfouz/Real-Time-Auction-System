using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Features.Orders.Commands.AddFeedback;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Sellers;
using Tests.Application.Features.Sellers;

namespace Tests.Application.Features.Orders.Events;

public class NotifySellerOnFeedbackLeftDomainEventHandlerTests : OrderBaseTest<NotifySellerOnFeedbackLeftDomainEventHandler>
{
    [Fact]
    public async Task Handle_SellerFound_SendsNotification()
    {
        // Arrange
        var domainEvent = OrderHelper.CreateFeedbackLeftEvent();
        var expectedSeller = SellerHelper.CreateValidSeller();

        _sellerRepository.GetByAuctionIdAsync(domainEvent.AuctionId, Arg.Any<CancellationToken>())
            .Returns(expectedSeller);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Target the correct command type and properties
        await _sender.Received(1).Send(
            Arg.Is<CreateNotificationCommand>(c => 
                c.UserId == expectedSeller.Id &&
                c.Method == NotificationMethods.ReceiveNotification
            ), 
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task Handle_SellerNotFound_SkipsNotification()
    {
       // Arrange
        var domainEvent = OrderHelper.CreateFeedbackLeftEvent();

        // Simulate database returning null
        _sellerRepository.GetByAuctionIdAsync(domainEvent.AuctionId, Arg.Any<CancellationToken>())
            .Returns((Seller?)null);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Properly clear arguments to completely ensure no call was attempted
        await _sender.DidNotReceiveWithAnyArgs().Send(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }
}