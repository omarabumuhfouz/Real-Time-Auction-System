using MazadZone.Domain.Auctions;

namespace MazadZone.Domain.Orders.Events;

public sealed record OrderConfirmedDomainEvent(OrderId OrderId, AuctionId AuctionId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
