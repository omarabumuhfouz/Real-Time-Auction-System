namespace MazadZone.Domain.Payments.ValueObjects;

[ValueObject<Guid>(conversions: Conversions.SystemTextJson | Conversions.TypeConverter )]
public partial struct TransactionId
{
    public static TransactionId New() => From(Guid.CreateVersion7());
}


