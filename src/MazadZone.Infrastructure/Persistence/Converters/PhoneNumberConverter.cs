using MazadZone.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class PhoneNumberConverter : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberConverter() 
        : base(
            phoneNumber => phoneNumber.Value,
            value => PhoneNumber.FromDatabase(value))
    {
    }
}