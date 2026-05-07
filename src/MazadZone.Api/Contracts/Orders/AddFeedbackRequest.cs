using MazadZone.Application.Features.Orders.Commands.AddFeedback;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Contracts.Orders;

public record AddFeedbackRequest(int Rating, string Comment)
{
    public AddFeedbackCommand ToCommand(OrderId orderId)
        => new (orderId, Rating, Comment);
}