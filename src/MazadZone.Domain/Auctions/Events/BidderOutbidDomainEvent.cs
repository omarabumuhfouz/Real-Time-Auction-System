namespace MazadZone.Domain.Auctions.Events;

public sealed record BidderOutbidDomainEvent(AuctionId AuctionId, BidId OutbidBidId, UserId OutbidBidderId, Money OutBidAmount) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}