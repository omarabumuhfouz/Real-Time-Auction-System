namespace MazadZone.Application.Features.Users.Commands.BulkBan;

public record BulkBanUsersCommand(List<UserId> UserIds, string Reason) : ICommand<Unit>;