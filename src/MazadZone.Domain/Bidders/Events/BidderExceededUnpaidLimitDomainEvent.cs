namespace MazadZone.Domain.Bidders.Events;

public record BidderExceededUnpaidLimitDomainEvent(
    UserId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}