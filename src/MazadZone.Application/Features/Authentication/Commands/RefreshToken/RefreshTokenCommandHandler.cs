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

    public async Task<Result<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var hashedOldToken = _tokenProvider.HashToken(request.RefreshToken);

        var user = await _userRepository.GetByRefreshTokenAsync(hashedOldToken, ct);

        if (user is null)
        {
            return UserErrors.InvalidToken;
        };

        // Generate the new token and hash it
        string newRefreshToken = _tokenProvider.GenerateRefreshToken();
        var hashedNewToken = _tokenProvider.HashToken(newRefreshToken);

        //  FIX: Pass the HASHED old token, not the raw one!
        var rotationResult = user.RotateRefreshToken(hashedOldToken, hashedNewToken);
        if (rotationResult.IsFailure) return rotationResult.TopError;

        //  Generate new access token and save
        string newAccessToken = _tokenProvider.GenerateAccessToken(user);
        await _unitOfWork.SaveChangesAsync(ct);

        // Return the RAW new refresh token to the user (never return the hash)
        return new TokenDto(newAccessToken, newRefreshToken);
    }
}