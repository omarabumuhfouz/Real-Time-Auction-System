namespace MazadZone.Features.DisputeTypes.Commands.Create;

public record CreateDisputeTypeCommand(string Name, string Description) : ICommand<Guid>;