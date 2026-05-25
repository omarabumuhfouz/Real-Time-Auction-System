[ValueObject<Guid>(conversions: Conversions.SystemTextJson | Conversions.TypeConverter)]
public partial struct DisputeTypeId
{
    public static DisputeTypeId New() => From(Guid.CreateVersion7());
    public static DisputeTypeId Empty => From(Guid.Empty);
}


