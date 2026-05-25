namespace MazadZone.Features.DisputeTypes.Commands.Restore;

public record RestoreDisputeTypeCommand(DisputeTypeId DisputeTypeId) : ICommand<Unit>;