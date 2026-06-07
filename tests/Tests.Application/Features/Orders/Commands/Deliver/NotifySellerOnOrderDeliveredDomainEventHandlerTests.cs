using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Features.Orders.Commands.Deliver;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Sellers;
using Tests.Application.Features.Sellers;

namespace Tests.Application.Features.Orders.Commands.Deliver;

public class NotifySellerOnOrderDeliveredDomainEventHandlerTests : OrderBaseTest<NotifySellerOnOrderDeliveredDomainEventHandler>
{
    [Fact]
    public async Task Handle_SellerNotFound_SkipsNotification()
    {
        // Arrange
        var domainEvent = OrderHelper.CreateOrderDeliveredEvent();

        _sellerRepository.GetByAuctionIdAsync(domainEvent.AuctionId, Arg.Any<CancellationToken>())
            .Returns((Seller?)null);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // We broadly verify that the Send method was never called with any parameters whatsoever.
        await _sender.DidNotReceiveWithAnyArgs().Send(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SellerFound_SendsNotification()
    {
        // Arrange
        var domainEvent = OrderHelper.CreateOrderDeliveredEvent();
        var seller = SellerHelper.CreateValidSeller();

        _sellerRepository.GetByAuctionIdAsync(domainEvent.AuctionId, Arg.Any<CancellationToken>())
            .Returns(seller);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // Verify the correct MediatR command was sent with the correct targeted Seller ID
        await _sender.Received(1).Send(
            Arg.Is<CreateNotificationCommand>(c => 
                c.UserId == seller.Id &&
                c.Method == NotificationMethods.ReceiveNotification
            ), 
            Arg.Any<CancellationToken>()
        );
    }
}