using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Features.Orders.Commands.Confirm;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Sellers;
using Tests.Application.Features.Sellers;

namespace Tests.Application.Features.Orders.Events;

public class NotifySellerOnOrderConfirmedDomainEventHandlerTests : OrderBaseTest<NotifySellerOnOrderConfirmedDomainEventHandler>
{
    [Fact]
    public async Task Handle_SellerNotFound_SkipsNotification()
    {
        // Arrange
        var domainEvent = OrderHelper.CreateOrderConfirmedEvent();

        // Simulate database returning null
        _sellerRepository.GetByAuctionIdAsync(domainEvent.AuctionId, Arg.Any<CancellationToken>())
            .Returns((Seller?)null);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Verify that _sender was never called to send any command
        await _sender.DidNotReceiveWithAnyArgs().Send(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SellerFound_SendsNotification()
    {
        // Arrange
        var domainEvent = OrderHelper.CreateOrderConfirmedEvent();
        var expectedSeller = SellerHelper.CreateValidSeller();

        _sellerRepository.GetByAuctionIdAsync(domainEvent.AuctionId, Arg.Any<CancellationToken>())
            .Returns(expectedSeller);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Verify the correct command type and targeted seller ID
        await _sender.Received(1).Send(
            Arg.Is<CreateNotificationCommand>(c => 
                c.UserId == expectedSeller.Id &&
                c.Method == NotificationMethods.ReceiveNotification
            ), 
            Arg.Any<CancellationToken>()
        );
    }
}