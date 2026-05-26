using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Categories;
using MazadZone.Domain.Disputes;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Sellers;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users;
using MazadZone.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using MzadZone.Domain.Payments;
using MzadZone.Domain.Payments.Entities;

namespace MazadZone.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<SigningKey> SigningKeys { get; set; } = null!;
    public DbSet<HashedRefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Feedback> Feedbacks { get; set; } = null!;
    public DbSet<Dispute> Disputes { get; set; } = null!;
    public DbSet<DisputeType> DisputeTypes { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Bidder> Bidders { get; set; } = null!;
    public DbSet<Seller> Sellers { get; set; } = null!;
    public DbSet<Auction> Auctions { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Bid> Bids { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Reason>();
        modelBuilder.Ignore<Resolution>();
        modelBuilder.Ignore<Rating>();
        modelBuilder.Ignore<Comment>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        // Apply safe value comparers to all properties wrapping Guid value objects (Vogen)
        // to prevent EF Core from throwing initialization exceptions on default values.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var type = property.ClrType;
                var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
                var valueProp = underlyingType.GetProperty("Value", typeof(Guid));
                if (valueProp != null && valueProp.DeclaringType == underlyingType)
                {
                    var comparerType = typeof(VogenValueComparer<>).MakeGenericType(underlyingType);
                    var comparerInstance = (Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer)Activator.CreateInstance(comparerType)!;
                    property.SetValueComparer(comparerInstance);
                }
            }
        }
    }
}

public class VogenValueComparer<TId> : Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<TId>
{
    private static readonly System.Reflection.PropertyInfo? ValueProperty = typeof(TId).GetProperty("Value", typeof(Guid));

    public VogenValueComparer() : base(
        (c1, c2) => SafeEquals(c1, c2),
        c => SafeGetHashCode(c),
        c => c)
    {
    }

    private static bool SafeEquals(TId? c1, TId? c2)
    {
        return GetGuid(c1) == GetGuid(c2);
    }

    private static int SafeGetHashCode(TId? instance)
    {
        return GetGuid(instance).GetHashCode();
    }

    private static Guid GetGuid(TId? instance)
    {
        if (instance is null)
            return Guid.Empty;

        if (ValueProperty == null)
            return Guid.Empty;

        try
        {
            var val = ValueProperty.GetValue(instance);
            return val is Guid g ? g : Guid.Empty;
        }
        catch
        {
            return Guid.Empty;
        }
    }
}