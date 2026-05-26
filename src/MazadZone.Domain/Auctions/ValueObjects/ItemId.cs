namespace MazadZone.Domain.Auctions.ValueObjects;


[ValueObject<Guid>(conversions: Conversions.SystemTextJson | Conversions.TypeConverter)]
public partial struct ItemId
{
    public static ItemId New() => From(Guid.CreateVersion7());
    public static ItemId Empty => From(Guid.Empty);
}