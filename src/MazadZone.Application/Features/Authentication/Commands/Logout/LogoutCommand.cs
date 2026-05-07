using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Authentication.Commands.Logout;

public record LogoutCommand(
     UserId UserId,
     string RefreshToken,
     bool IsLogoutFromAllDevices

) : ICommand<Unit>;