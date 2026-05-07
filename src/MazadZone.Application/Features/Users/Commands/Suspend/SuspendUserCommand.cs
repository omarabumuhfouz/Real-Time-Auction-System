using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.Suspend;

public record SuspendUserCommand(UserId UserId, DateTime Until) : ICommand<Unit>;