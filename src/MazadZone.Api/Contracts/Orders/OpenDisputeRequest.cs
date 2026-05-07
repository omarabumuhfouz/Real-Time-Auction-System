using MazadZone.Application.Features.Orders.Commands.OpenDispute;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Contracts.Orders;

public record OpenDisputeRequest(string Reason)
{
    public OpenDisputeCommand ToCommand(OrderId orderId)
        =>  new OpenDisputeCommand(orderId, Reason);
}