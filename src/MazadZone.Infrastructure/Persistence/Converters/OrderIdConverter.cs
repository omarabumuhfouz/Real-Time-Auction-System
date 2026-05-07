using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class OrderIdConverter : ValueConverter<OrderId, Guid>
{
    public OrderIdConverter() 
        : base(id => id.Value, guid =>  OrderId.From(guid)) { }
}

public class UserIdIdConverter : ValueConverter<UserId, Guid>
{
    public UserIdIdConverter() 
        : base(id => id.Value, guid =>  UserId.From(guid)) { }
}
