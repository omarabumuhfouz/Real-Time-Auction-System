using MazadZone.Domain.Auctions;

namespace MazadZone.Domain.Orders.Events;

public sealed record FeedbackLeftDomainEvent(OrderId OrderId, AuctionId AuctionId, int Rating, string Comment) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOnUtc => DateTime.UtcNow;
}