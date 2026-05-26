using MazadZone.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class PaymentMethodIdConverter : ValueConverter<PaymentMethodId, Guid>
{
    public PaymentMethodIdConverter()
        : base(id => id.Value, guid => PaymentMethodId.From(guid)) { }
}
