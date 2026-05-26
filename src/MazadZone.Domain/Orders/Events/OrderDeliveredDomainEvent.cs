using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Orders.Events;

public sealed record OrderDeliveredDomainEvent(OrderId OrderId, AuctionId AuctionId, UserId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
