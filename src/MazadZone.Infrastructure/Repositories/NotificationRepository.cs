using MazadZone.Domain.Notifications;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Infrastructure.Persistence;

namespace MazadZone.Infrastructure.Repositories;

public class NotificationRepository : GenericRepository<Notification, NotificationId>, INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<IEnumerable<Notification>> GetByUserIdAsync(UserId userId, CancellationToken ct = default)
    {
        Console.WriteLine($"[MOCK DB] Fetching notifications for User ID: {userId.Value}");
        // Return an empty list so the caller doesn't throw a NullReferenceException
        return Task.FromResult<IEnumerable<Notification>>([]); 
    }

    public Task<int> GetUnreadCountAsync(UserId userId, CancellationToken ct = default)
    {
        Console.WriteLine($"[MOCK DB] Fetching unread notification count for User ID: {userId.Value}");
        return Task.FromResult(0);
    }

    public Task NotifyAdminAsync(string title, string message, CancellationToken ct = default)
    {
        Console.WriteLine("\n==========================================================");
        Console.WriteLine(" 🔔 MOCK NOTIFICATION SAVED (ROLE: ADMIN)");
        Console.WriteLine("==========================================================");
        Console.WriteLine($" TITLE:   {title}");
        Console.WriteLine($" MESSAGE: {message}");
        Console.WriteLine("==========================================================\n");
        
        return Task.CompletedTask;
    }

    public Task NotifyBidderAsync(Guid bidderId, string title, string message, CancellationToken ct = default)
    {
        Console.WriteLine("\n==========================================================");
        Console.WriteLine(" 🔔 MOCK NOTIFICATION SAVED (ROLE: BIDDER)");
        Console.WriteLine("==========================================================");
        Console.WriteLine($" TO (Guid): {bidderId}");
        Console.WriteLine($" TITLE:     {title}");
        Console.WriteLine($" MESSAGE:   {message}");
        Console.WriteLine("==========================================================\n");
        
        return Task.CompletedTask;
    }

    public Task NotifySellerAsync(Guid sellerId, string title, string message, CancellationToken ct = default)
    {
        Console.WriteLine("\n==========================================================");
        Console.WriteLine(" 🔔 MOCK NOTIFICATION SAVED (ROLE: SELLER)");
        Console.WriteLine("==========================================================");
        Console.WriteLine($" TO (Guid): {sellerId}");
        Console.WriteLine($" TITLE:     {title}");
        Console.WriteLine($" MESSAGE:   {message}");
        Console.WriteLine("==========================================================\n");
        
        return Task.CompletedTask;
    }

    public Task NotifyUserAsync(UserId userId, string title, string message, CancellationToken ct)
    {
        Console.WriteLine("\n==========================================================");
        Console.WriteLine(" 🔔 MOCK NOTIFICATION SAVED (ROLE: USER)");
        Console.WriteLine("==========================================================");
        Console.WriteLine($" TO (UserId): {userId.Value}");
        Console.WriteLine($" TITLE:       {title}");
        Console.WriteLine($" MESSAGE:     {message}");
        Console.WriteLine("==========================================================\n");
        
        return Task.CompletedTask;
    }
}