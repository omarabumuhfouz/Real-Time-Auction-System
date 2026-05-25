namespace MazadZone.Features.DisputeTypes.Commands.Delete;

public record DeleteDisputeTypeCommand(DisputeTypeId DisputeTypeId) : ICommand<Unit>;