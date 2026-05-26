using MazadZone.Domain.Bidders;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Orders.Events;

public sealed record OrderShippedDomainEvent(OrderId OrderId,UserId BidderId) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}
