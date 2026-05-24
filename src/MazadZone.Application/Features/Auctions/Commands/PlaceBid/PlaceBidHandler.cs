using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Repositories;
using MazadZone.Application.Common.Exceptions;

namespace MazadZone.Application.Features.Auctions.Commands.PlaceBid;

public class PlaceBidHandler(
    IAuctionRepository _auctionRepository,
    IPaymentService _paymentService,
    IUnitOfWork unitOfWork,
    IDateTimeProvider _dateTimeProvider,
    ILogger<PlaceBidHandler> _logger
) : ICommandHandler<PlaceBidCommand, Unit>
{
    public async Task<Result<Unit>> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
    {
        PlaceBidLog.LogPlaceBidAttempt(_logger, request.AuctionId.Value, request.BidderId.Value, request.Amount.Amount);

        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId, cancellationToken);

        if (auction is null)
        {
            PlaceBidLog.LogAuctionNotFound(_logger, request.AuctionId.Value);
            return Result.Failure<Unit>(AuctionErrors.NotFound);
        }

        // 1. (Pre-check)
        var checkPlaceBidResult = auction.CheckPlaceBid(request.BidderId, request.Amount.Amount, _dateTimeProvider.Now);
        if (checkPlaceBidResult.IsFailure)
        {
            PlaceBidLog.LogDomainViolation(_logger, request.AuctionId.Value, checkPlaceBidResult.TopError.Message);
            return Result.Failure<Unit>(checkPlaceBidResult.TopError);
        }

        // 2. connect with payment service
        string authHoldId = await _paymentService.AuthorizeAsync(
            userId: request.BidderId.Value, 
            auctionId: request.AuctionId.Value, 
            methodId: request.PaymentMethodId, 
            depositAmount: checkPlaceBidResult.Value.DepositAmount, 
            cancellationToken: cancellationToken
        );

        if (string.IsNullOrEmpty(authHoldId))
        {
            PlaceBidLog.LogPaymentAuthorizationFailed(_logger, request.AuctionId.Value, request.BidderId.Value);
            return Result.Failure<Unit>(AuctionErrors.PaymentAuthorizationFailed);
        }

        // 3. execute in domain
        var placeBidResult = auction.PlaceVerifiedBid(checkPlaceBidResult.Value, authHoldId, _dateTimeProvider.Now);
        
        // if the domain reject we do unauthorize
        if (placeBidResult.IsFailure)
        {
            await _paymentService.UnAuthorizeAsync(authHoldId, cancellationToken); // التراجع المالي
            PlaceBidLog.LogDomainViolation(_logger, request.AuctionId.Value, placeBidResult.TopError.Message);
            return Result.Failure<Unit>(placeBidResult.TopError);
        }

        // 4. save in database and catch concurency problem
        try
        {
            _auctionRepository.Update(auction); 
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException) 
        {
            // conflect : if the another user do bids in the same time 
            await _paymentService.UnAuthorizeAsync(authHoldId, cancellationToken); // unautherize
            PlaceBidLog.LogPersistenceFailed(_logger, request.AuctionId.Value, request.BidderId.Value, "Concurrency conflict: You were outbid during processing.");
            return Result.Failure<Unit>(AuctionErrors.ConcurrencyConflict);
        }
        catch (Exception ex)
        {
            // another exptions as inconnect database 
            await _paymentService.UnAuthorizeAsync(authHoldId, cancellationToken); // unauthorize
            PlaceBidLog.LogPersistenceFailed(_logger, request.AuctionId.Value, request.BidderId.Value, ex.Message);
            return Result.Failure<Unit>(AuctionErrors.DatabaseError);
        }

        PlaceBidLog.LogSuccess(_logger, request.AuctionId.Value, request.BidderId.Value);
        return Result.Success(Unit.Value);
    }
}