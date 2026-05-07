using MazadZone.Application.Features.Authentication.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, TokenDto>
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _logger = logger;
    }

    async Task<Result<TokenDto>> IRequestHandler<RefreshTokenCommand, Result<TokenDto>>.Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var hashedToken = _tokenProvider.HashToken(request.RefreshToken);

        var user = await _userRepository.GetByRefreshTokenAsync(hashedToken, ct);

        if (user is null)
        {
            return UserErrors.InvalidToken;
        }

        string newRefreshToken = _tokenProvider.GenerateRefreshToken();
        var hashRefreshToken = _tokenProvider.HashToken(newRefreshToken);
        var rotationResult = user.RotateRefreshToken(request.RefreshToken, hashRefreshToken);

        if (rotationResult.IsFailure) return rotationResult.TopError;

        string newAccessToken = _tokenProvider.GenerateAccessToken(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return new TokenDto(newAccessToken, newRefreshToken);
    }
}