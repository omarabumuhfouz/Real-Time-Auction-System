using MazadZone.Application.Features.Authentication.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Commands.BecomeSeller;

public sealed class BecomeSellerCommandHandler : ICommandHandler<BecomeSellerCommand, TokenDto>
{
    private readonly ISellerRepository _sellerRepository;
    private readonly IBidderRepository _bidderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<BecomeSellerCommandHandler> _logger;


    public BecomeSellerCommandHandler(
        ISellerRepository sellerRepository,
        IBidderRepository bidderRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenProvider tokenProvider,
        ILogger<BecomeSellerCommandHandler> logger
    )
    {
        _sellerRepository = sellerRepository;
        _bidderRepository = bidderRepository;
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TokenDto>> Handle(BecomeSellerCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithTokensAsync(request.UserId, ct);  
        if(user is null)
        {
            GlobalLogs.LogUserNotFound(_logger,request.UserId);
            return BidderErrors.NotFound; ;
        }

        var nationalId = await _bidderRepository.GetNationalIdByBidderIdAsync(request.UserId, ct);
        if(nationalId is null)
        {
            GlobalLogs.LogBidderNotFound(_logger, request.UserId);
            return BidderErrors.NotFound;
        }

        var result = Seller.BecomeSeller(request.UserId);

        if (result.IsFailure)
        {
            BecomeSellerLogs.LogDomainRuleViolation(_logger, request.UserId, result.TopError.Code);
            return result.TopError;
        }

        user.AddSellerRole();

        var accessToken = _tokenProvider.GenerateAccessToken(user);
        var refreshTokenRaw = _tokenProvider.GenerateRefreshToken();
        var hashedRefreshToken = _tokenProvider.HashToken(refreshTokenRaw);

        user.AddRefreshToken(hashedRefreshToken);
        var seller = result.Value;

        _sellerRepository.Add(seller);

        await _unitOfWork.SaveChangesAsync(ct);

        BecomeSellerLogs.LogSuccess(_logger, request.UserId);

        return new TokenDto(accessToken, refreshTokenRaw);
    }
}