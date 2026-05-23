using Microsoft.AspNetCore.SignalR;


namespace MazadZone.Infrastructure.RealTime.Hubs;

public class NotificationsHub : Hub
{
    public const string HubUrl = "/hubs/Notifications";
}