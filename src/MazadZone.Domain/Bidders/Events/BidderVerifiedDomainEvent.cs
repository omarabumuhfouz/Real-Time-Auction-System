
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Bidders.Events;

public record BidderVerifiedDomainEvent(UserId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}