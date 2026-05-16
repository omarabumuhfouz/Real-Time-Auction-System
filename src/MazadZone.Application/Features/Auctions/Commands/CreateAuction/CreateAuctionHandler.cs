using MazadZone.Domain.Auctions;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.ValueObjects;

namespace MazadZone.Application.Features.Auctions.Commands.CreateAuction;
/// <summary>
/// Handles the creation of a new auction.
/// </summary>
/// <typeparam name="CreateAuctionHandler"></typeparam>
public class CreateAuctionHandler
(
    IAuctionRepository _auctionRepository,
    IUnitOfWork _unitOfWork,
    ILogger<CreateAuctionHandler> _logger
): ICommandHandler<CreateAuctionCommand, AuctionId>
{
    public async Task<Result<AuctionId>> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
    {
        CreateAuctionLog.LogHandlingCreateAuction(_logger, request.ItemId.Value.ToString());

        var createResult = Auction.Create(
            request.ItemId,
            request.SellerId,
            request.ShippingAddress,
            request.StartBid,
            request.MinBidAmount,
            Currency.Jod,
            request.StartTime,
            request.EndTime);

        if (createResult.IsFailure) {
            CreateAuctionLog.LogDomainViolation(_logger, request.ItemId.Value.ToString(), createResult.TopError.Message);
            return Result.Failure<AuctionId>(createResult.TopError);
        }

        var auction = createResult.Value;

        _auctionRepository.Add(auction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        CreateAuctionLog.LogAuctionCreated(_logger, request.ItemId.Value.ToString(), auction.Id.Value);

        return Result.Success(auction.Id);
        
    }
}