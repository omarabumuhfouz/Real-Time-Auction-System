using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Domain.Notifications;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class OrderIdConverter : ValueConverter<OrderId, Guid>
{
    public OrderIdConverter() 
        : base(id => id.Value, guid =>  OrderId.From(guid)) { }
}

public class HashedRefreshTokenIdConverter : ValueConverter<HashedRefreshTokenId, Guid>
{
    public HashedRefreshTokenIdConverter() : base(id => id.Value, guid =>  HashedRefreshTokenId.From(guid)) { }
}

public class NotificationIdConverter : ValueConverter<NotificationId, Guid>
{
    public NotificationIdConverter() 
        : base(id => id.Value, guid =>  NotificationId.From(guid)) { }
}