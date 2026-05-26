namespace MazadZone.Application.Features.Orders.Commands.ResolveDispute;

public record ResolveDisputeCommand(DisputeId DisputeId, string Resolution) : ICommand<Unit>;