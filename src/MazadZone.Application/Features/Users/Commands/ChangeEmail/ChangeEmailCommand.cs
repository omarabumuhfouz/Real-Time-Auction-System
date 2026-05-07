using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.ChangeEmail;

public record ChangeEmailCommand(UserId UserId, string NewEmail) : ICommand<Unit>;
