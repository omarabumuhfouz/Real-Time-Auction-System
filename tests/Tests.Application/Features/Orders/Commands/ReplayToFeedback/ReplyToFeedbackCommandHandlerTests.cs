using MazadZone.Application.Features.Orders.Commands.ReplyToFeedback;
using MazadZone.Domain.Orders;

namespace Tests.Application.Features.Orders.Commands.ReplyToFeedback;

public class ReplyToFeedbackCommandHandlerTests : OrderBaseTest<ReplyToFeedbackCommandHandler>
{
    [Fact]
    public async Task Handle_OrderDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = new ReplyToFeedbackCommand(OrderId.New(), "Thank you for the feedback!");

        _orderRepository.GetByIdAsync(command.OrderId.Value, Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.NotFound);

        // Verify database writes were completely bypassed
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_OrderRejectsReply_ReturnsDomainError()
    {
        // Arrange
        var order = OrderHelper.CreateDeliveredOrder();  // not have any feedback yet, so replying would be invalid
        var command = new ReplyToFeedbackCommand(order.Id, "Thank you!");

        _orderRepository.GetByIdAsync(command.OrderId.Value, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        result.TopError.Code.ShouldNotBe(OrderErrors.NotFound.Code);

        // Verify the unit of work was not committed
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsReplyAndSavesChanges()
    {
        // Arrange
        // Create an order that already has feedback attached, so replying is a valid operation
        var order = OrderHelper.CreateOrderWithFeedback(); 
        var replyText = "Thank you for the kind words, enjoy the item!";
        var command = new ReplyToFeedbackCommand(order.Id, replyText);

        _orderRepository.GetByIdAsync(command.OrderId.Value, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        order.Feedback.Reply.ShouldNotBeNull();
        order.Feedback.Reply.Value.ShouldBe(replyText);

        // Verify the transaction was committed
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}