using Microsoft.AspNetCore.SignalR;

namespace MazadZone.Infrastructure.RealTime.Hubs;

public class AuctionsHub : Hub
{
    public const string HubUrl = "/hubs/Auctions";
}