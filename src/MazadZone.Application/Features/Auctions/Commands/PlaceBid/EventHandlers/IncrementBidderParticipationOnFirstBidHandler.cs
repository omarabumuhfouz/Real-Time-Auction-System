using MazadZone.Domain.Auctions.Events;
using MazadZone.Domain.Repositories; 

namespace MazadZone.Application.Auctions.EventHandlers;

public sealed class IncrementBidderParticipationOnFirstBidHandler 
    : INotificationHandler<BidPlacedDomainEvent>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IBidderRepository _bidderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IncrementBidderParticipationOnFirstBidHandler(IAuctionRepository auctionRepository, IBidderRepository bidderRepository, IUnitOfWork unitOfWork)
    {
        _auctionRepository = auctionRepository;
        _bidderRepository = bidderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BidPlacedDomainEvent notification, CancellationToken ct)
{
    // High-performance DB check instead of processing collections in C# memory
    bool hasAlreadyParticipated = await _auctionRepository.HasBidderAlreadyParticipatedAsync(
        notification.AuctionId,
        notification.BidderId,
        notification.BidId,
        ct);

    if (hasAlreadyParticipated)
    {
        return; // Already a participant
    }

        // Process the bidder counter mutation...
        var bidderProfile = await _bidderRepository.GetByIdAsync(notification.BidderId, ct);
        if (bidderProfile is null) return;

        bidderProfile.RecordAuctionParticipated();
        await _unitOfWork.SaveChangesAsync(ct);
}
}