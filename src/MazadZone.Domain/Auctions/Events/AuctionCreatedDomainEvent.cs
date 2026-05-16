
using MazadZone.Domain.Users;

namespace MazadZone.Domain.Auctions.Events;

public sealed record AuctionCreatedDomainEvent(AuctionId auctionId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
