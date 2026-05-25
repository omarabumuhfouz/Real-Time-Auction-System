namespace MazadZone.Domain.Bidders.Events;

// Triggered when the shipping address changes (Useful if there are active shipments)
public record BidderAddressUpdatedDomainEvent(
    UserId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.Now;
}