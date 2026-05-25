namespace MazadZone.Domain.Orders.Events;

public sealed record OrderCreatedDomainEvent(OrderId OrderId, UserId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
