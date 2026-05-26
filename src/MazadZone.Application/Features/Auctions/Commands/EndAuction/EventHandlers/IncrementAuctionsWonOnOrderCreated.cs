namespace MazadZone.Application.Features.Auctions.Commands.EndAuction.EventHandlers;

using MediatR;
using Microsoft.Extensions.Logging;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Auctions.Events;

public sealed class IncrementAuctionsWonOnOrderCreated : INotificationHandler<AuctionEndedDomainEvent>
{
    private readonly IBidderRepository _bidderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IncrementAuctionsWonOnOrderCreated> _logger;
    private readonly IAuctionRepository _auctionRepository;

    public IncrementAuctionsWonOnOrderCreated(
        IBidderRepository bidderRepository,
        IUnitOfWork unitOfWork,
        ILogger<IncrementAuctionsWonOnOrderCreated> logger,
        IAuctionRepository auctionRepository
    )
    {
        _bidderRepository = bidderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _auctionRepository = auctionRepository;
    }

    public async Task Handle(AuctionEndedDomainEvent notification, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdWithBidsAsync(notification.AuctionId, cancellationToken);
        if (auction is null) return;

        var bidderId = auction.CurrentLeadingBid?.BidderId;
        if (bidderId is null) return;



        _logger.LogInformation(
            "Processing win metrics calculation for Bidder: {BidderId}", 
            bidderId);

        // Fetch the profile aggregate tracking entity using the Buyer's UserId
        var bidder = await _bidderRepository.GetByIdAsync(bidderId.Value, cancellationToken);

        if (bidder is null)
        {
            _logger.LogWarning(
                "Profile metrics aggregate not found for winning user: {BuyerId}. Analytics increment bypassed.", 
                bidderId);
            return;
        }

        // Execute encapsulated domain mutation logic safely 
        bidder.RecordAuctionWon();

        // Persist the updated state to the data store
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully incremented AuctionsWon for User: {BuyerId}.", 
            bidderId);
    }
}