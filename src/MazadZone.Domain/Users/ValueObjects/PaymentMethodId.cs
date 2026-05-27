using Vogen;

namespace MazadZone.Domain.Users.ValueObjects;

[ValueObject<Guid>(conversions: Conversions.SystemTextJson | Conversions.TypeConverter)]
public partial struct PaymentMethodId
{
    public static PaymentMethodId New() => From(Guid.CreateVersion7());
}
