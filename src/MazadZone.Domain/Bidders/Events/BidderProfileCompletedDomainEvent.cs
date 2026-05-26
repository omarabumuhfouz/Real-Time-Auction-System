namespace MazadZone.Domain.Bidders.Events;

public record BidderProfileCompletedDomainEvent(
    UserId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
