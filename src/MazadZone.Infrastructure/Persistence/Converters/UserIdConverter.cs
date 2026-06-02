using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class UserIdConverter : ValueConverter<UserId, Guid>
{
    public UserIdConverter() 
        : base(id => id.Value, guid =>  UserId.From(guid)) { }
}
