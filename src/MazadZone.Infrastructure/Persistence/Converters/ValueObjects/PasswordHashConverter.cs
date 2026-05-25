using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.ValueObjects; // Update namespace

namespace MazadZone.Infrastructure.Persistence.Converters;

public class PasswordHashConverter : ValueConverter<PasswordHash, string>
{
    public PasswordHashConverter() 
        : base(
            passwordHash => passwordHash.Value,
            value => PasswordHash.FromDatabase(value))
    {
    }
}