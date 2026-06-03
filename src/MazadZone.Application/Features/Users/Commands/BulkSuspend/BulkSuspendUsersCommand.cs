namespace MazadZone.Application.Features.Users.Commands.BulkSuspend;

public record BulkSuspendUsersCommand(List<UserId> UserIds, string Reason, DateTime Until) : ICommand<Unit>;