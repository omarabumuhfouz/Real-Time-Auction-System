using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public interface IRealTimeNotificationService : IScopedService
{
    Task SendNotificationAsync(UserNotificationDto userNotificationDto, CancellationToken cancellationToken = default);   
}