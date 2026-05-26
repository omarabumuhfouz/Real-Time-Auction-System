namespace MazadZone.Application.Features.Orders.Commands.ResolveDispute;

public record UnderReviewDisputeCommand(DisputeId DisputeId) : ICommand<Unit>;