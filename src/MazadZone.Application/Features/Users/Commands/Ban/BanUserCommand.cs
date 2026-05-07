using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.Ban;

public record BanUserCommand(UserId UserId, string Reason) : ICommand<Unit>;