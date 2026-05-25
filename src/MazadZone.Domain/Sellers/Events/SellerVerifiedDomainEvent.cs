using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Sellers.Events;

public sealed record SellerVerifiedDomainEvent(UserId SellerId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
