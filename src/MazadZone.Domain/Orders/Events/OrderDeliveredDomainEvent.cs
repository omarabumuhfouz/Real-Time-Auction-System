using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;

namespace MazadZone.Domain.Orders.Events;

public sealed record OrderDeliveredDomainEvent(OrderId OrderId, AuctionId AuctionId, BidderId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
