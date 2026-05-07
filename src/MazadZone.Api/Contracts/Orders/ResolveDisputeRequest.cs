using MazadZone.Application.Features.Orders.Commands.ResolveDispute;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Contracts.Orders;

public record ResolveDisputeRequest(string Resolution)
{
    public ResolveDisputeCommand ToCommand(OrderId orderId)
        =>  new ResolveDisputeCommand(orderId, Resolution);

}