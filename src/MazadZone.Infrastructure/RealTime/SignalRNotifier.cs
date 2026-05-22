using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Notifications;
using MazadZone.Infrastructure.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MazadZone.Infrastructure.RealTime;

/// <summary>
/// Implements the IRealTimeNotificationService using SignalR to send real-time notifications to users.
/// </summary>

public class SignalRNotifier
(
    IHubContext<NotificationsHub> _hubContext
): IRealTimeNotificationService
{
    public async Task SendNotificationAsync(UserNotificationDto userNotificationDto, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userNotificationDto.userId.ToString()).SendAsync(userNotificationDto.method, userNotificationDto, cancellationToken);
    }
}