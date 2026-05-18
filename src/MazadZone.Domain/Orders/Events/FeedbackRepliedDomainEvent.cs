using MazadZone.Domain.Auctions;

namespace MazadZone.Domain.Orders.Events;

public sealed record FeedbackRepliedDomainEvent(OrderId OrderId,  BidderId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}