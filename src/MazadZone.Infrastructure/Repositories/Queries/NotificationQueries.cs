using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Notifications.Queries.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Repositories.Queries;

public class NotificationQueries(AppDbContext context) : INotificationQueries
{
    private readonly AppDbContext _context = context;

    public async Task<NotificationDto?> GetNotificationByIdAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.Id == id)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.UserId, n.CreatedOnUtc, n.IsRead, n.ModifiedOnUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedList<NotificationDto>> GetNotificationsAsync(UserId userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderByDescending(n => n.CreatedOnUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.UserId, n.CreatedOnUtc, n.IsRead, n.ModifiedOnUtc))
            .ToListAsync(cancellationToken);

        return new PagedList<NotificationDto>(items, pageNumber, pageSize, totalCount);
    }

    public async Task<int> GetUnreadCountAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }
}