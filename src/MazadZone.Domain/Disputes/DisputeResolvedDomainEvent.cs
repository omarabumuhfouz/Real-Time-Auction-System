namespace MazadZone.Domain.Disputes;

public sealed record DisputeResolvedDomainEvent( DisputeId DisputeId, OrderId OrderId,string Resolution) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}