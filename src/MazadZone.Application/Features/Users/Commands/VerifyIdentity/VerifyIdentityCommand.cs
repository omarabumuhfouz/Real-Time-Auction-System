using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.VerifyIdentity;

public record VerifyIdentityCommand(UserId UserId, byte[] ImageBytes) : ICommand<Unit>;
