using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

/// <summary>
/// Service interface for broadcasting real-time auction updates to clients.
/// </summary>
public interface IAuctionStreamService : IScopedService
{
    Task BroadcastAuctionUpdateAsync<T>(string method, T updateData, CancellationToken cancellationToken = default);

}