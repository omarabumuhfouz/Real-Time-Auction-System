namespace MazadZone.Features.DisputeTypes.Commands.Update;

public record UpdateDisputeTypeCommand(DisputeTypeId DisputeTypeId, string Name, string Description) : ICommand<Unit>;