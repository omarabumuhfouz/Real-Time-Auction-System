
using MazadZone.Domain.Users;

namespace MazadZone.Domain.Auctions.Events;

public sealed record AuctionCreatedDomainEvent(AuctionId AuctionId, UserId SellerId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
