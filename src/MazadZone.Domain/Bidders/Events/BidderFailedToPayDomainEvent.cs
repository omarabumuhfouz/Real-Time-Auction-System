using MazadZone.Domain.Auctions;

namespace MazadZone.Domain.Bidders.Events;

public record BidderFailedToPayDomainEvent(
    UserId BidderId,
    AuctionId AuctionId,
    int CurrentUnpaidCount) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}