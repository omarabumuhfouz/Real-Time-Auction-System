namespace MazadZone.Domain.Users.ValueObjects;

[ValueObject<Guid>(conversions: Conversions.SystemTextJson | Conversions.TypeConverter )]
public partial struct HashedRefreshTokenId
{
    public static HashedRefreshTokenId New() => From(Guid.CreateVersion7());
}


