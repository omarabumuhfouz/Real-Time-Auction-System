using MazadZone.Application.Services;
using MazadZone.Infrastructure.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MazadZone.Infrastructure.RealTime;
/// <summary>
/// Service for broadcasting real-time auction updates to clients using SignalR.
/// </summary>
public class AuctionStreamService(
    IHubContext<AuctionsHub> _hubContext
    ) : IAuctionStreamService
{

    public async Task BroadcastAuctionUpdateAsync<T>(string method, T updateData, CancellationToken cancellationToken = default)
    {
        // Broadcast the update to all connected clients in the AuctionsHub using the specified method name and update data
        await _hubContext.Clients.All.SendAsync(method, updateData, cancellationToken);
    }
}