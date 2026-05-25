namespace MazadZone.Domain.Orders.Events;

public sealed record DisputeOpenedDomainEvent( DisputeId DisputeId, OrderId OrderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
