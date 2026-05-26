using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Sellers.Events;

public sealed record SellerRatingUpdatedDomainEvent(UserId SellerId, decimal NewRating) : IDomainEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOnUtc => DateTime.UtcNow;
}