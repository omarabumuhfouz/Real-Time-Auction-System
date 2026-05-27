namespace MazadZone.Application.Features.Users.Commands.BulkActivate;

public record BulkActivateUsersCommand(IReadOnlyCollection<UserId> UserIds) : ICommand<Unit>;