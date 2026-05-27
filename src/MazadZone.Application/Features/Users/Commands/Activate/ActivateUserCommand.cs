namespace MazadZone.Application.Features.Users.Commands.Activate;

public record ActivateUserCommand(UserId UserId) : ICommand<Unit>;