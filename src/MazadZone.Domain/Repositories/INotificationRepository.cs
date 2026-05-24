using MazadZone.Domain.Auctions;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Repositories;

public interface INotificationRepository : IGenericRepository<Notification, NotificationId>, IScopedService
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(UserId userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(UserId userId, CancellationToken ct = default);
    Task NotifyBidderAsync(Guid bidderId, string title, string message, CancellationToken ct = default);
    Task NotifySellerAsync(Guid sellerId, string title, string message, CancellationToken ct = default);
    Task NotifyAdminAsync(string title, string message, CancellationToken ct = default);
    Task NotifyUserAsync(UserId userId, string title, string message, CancellationToken ct);

}