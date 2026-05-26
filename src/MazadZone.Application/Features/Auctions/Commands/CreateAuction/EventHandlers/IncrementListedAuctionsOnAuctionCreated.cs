namespace MazadZone.Application.Features.Auctions.EventHandlers;

using MediatR;
using MazadZone.Domain.Sellers;
using MazadZone.Domain.Auctions.Events;
using Microsoft.Extensions.Logging;
using MazadZone.Domain.Repositories;

public sealed class IncrementListedAuctionsOnAuctionCreated : INotificationHandler<AuctionCreatedDomainEvent>
{
    private readonly ISellerRepository _sellerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IncrementListedAuctionsOnAuctionCreated> _logger;

    public IncrementListedAuctionsOnAuctionCreated(ISellerRepository sellerRepository, IUnitOfWork unitOfWork, ILogger<IncrementListedAuctionsOnAuctionCreated> logger)
    {
        _sellerRepository = sellerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(AuctionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing listing metrics update for Seller: {SellerId} from Auction: {AuctionId}", 
            notification.SellerId, 
            notification.AuctionId);

        // Fetch the profile aggregate tracking entity using the Seller's UserId
        var profile = await _sellerRepository.GetByIdAsync(notification.SellerId, cancellationToken);

        if (profile is null)
        {
            _logger.LogWarning(
                "Profile metrics aggregate not found for listing user: {SellerId}. Analytics update bypassed.", 
                notification.SellerId);
            return;
        }

        // Execute encapsulated domain mutation logic safely 
        profile.RecordListedAuction();

        // Persist the updated state to the data store
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully incremented ListedAuctionsCount for User: {SellerId}.", 
            notification.SellerId);
    }
}