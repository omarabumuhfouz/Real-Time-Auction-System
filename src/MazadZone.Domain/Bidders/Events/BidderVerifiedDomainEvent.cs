
namespace MazadZone.Domain.Bidders.Events;

public record BidderVerifiedDomainEvent(BidderId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}