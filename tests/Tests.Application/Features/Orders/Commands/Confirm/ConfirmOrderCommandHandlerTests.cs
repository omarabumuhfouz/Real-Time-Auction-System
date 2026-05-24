using MazadZone.Application.Features.Orders.Commands.Confirm;
using MazadZone.Application.Features.Payments.Commands.PayRemainingAmount;
using MazadZone.Domain.Orders;
using MediatR;

namespace Tests.Application.Features.Orders.Commands.Confirm;

public class ConfirmOrderCommandHandlerTests : OrderBaseTest<ConfirmOrderCommandHandler>
{
    [Fact]
    public async Task Handle_OrderDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = OrderHelper.CreateConfirmOrderCommand();

        _orderRepository.GetByIdAsync(command.OrderId, Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.NotFound);

        // Verify payment logic was completely bypassed
        // await _sender.DidNotReceive().Send(Arg.Any<PayRemainingAmountCommand>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task Handle_ConfirmFails_ReturnsDomainError()
    {
        // Arrange - Create a fresh order and force an invalid status transition
        var order = OrderHelper.CreateCancelledOrder();  // Cancelled orders cannot transition to Confirmed

        var command = OrderHelper.CreateConfirmOrderCommand(order.Id);

        _orderRepository.GetByIdAsync(command.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);


        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.CannotConfirm);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ConfirmsOrderAndSavesChanges()
    {
        // Arrange
        var order = OrderHelper.CreatePendingOrder(); // Start with a valid pending order
        var command = OrderHelper.CreateConfirmOrderCommand(order.Id);

        _orderRepository.GetByIdAsync(command.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);

        // Assert state mutation actually executed on aggregate root
        order.Status.ShouldBe(OrderStatus.Confirmed);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

}