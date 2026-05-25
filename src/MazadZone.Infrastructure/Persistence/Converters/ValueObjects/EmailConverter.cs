using MazadZone.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter() 
        : base(
            email => email.Value,
            value => Email.FromDatabase(value))
    {
    }
}