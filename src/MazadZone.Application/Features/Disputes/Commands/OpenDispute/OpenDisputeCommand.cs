namespace MazadZone.Application.Features.Disputes.Commands.OpenDispute;

public record OpenDisputeCommand(OrderId OrderId,DisputeTypeId DisputeTypeId, string Title, string Description, List<Image>? Images = null) : ICommand<Unit>;