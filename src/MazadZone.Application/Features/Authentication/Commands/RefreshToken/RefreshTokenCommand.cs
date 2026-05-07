using MazadZone.Application.Features.Authentication.DTOs;

namespace MazadZone.Application.Features.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(
 string RefreshToken
)
 : ICommand<TokenDto>;