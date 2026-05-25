namespace MazadZone.Domain.Notifications;

[ValueObject<Guid>(conversions: Conversions.SystemTextJson | Conversions.TypeConverter )]
public partial struct NotificationId
{
    public static NotificationId New() => From(Guid.CreateVersion7());
    public static NotificationId Empty => From(Guid.Empty);
}




