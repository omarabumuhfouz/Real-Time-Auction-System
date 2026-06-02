using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Infrastructure.Persistence;
using MazadZone.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User,UserId>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken ct)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email), ct);
    }

    public Task<User?> GetByIdWithTokensAsync(UserId id, CancellationToken ct)
    {
        return _context.Users
             .Include(u => u.HashedRefreshTokens)
             .FindByIdAsync(id, ct);
    }

    public Task<User?> GetByIdWithPaymentMethodsAsync(UserId id, CancellationToken ct)
    {
        return _context.Users
             .Include(u => u.PaymentMethods)
             .FindByIdAsync(id, ct);
    }

    public Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        return _context.Users
        .Include(u => u.HashedRefreshTokens)
        .FirstOrDefaultAsync(u => u.HashedRefreshTokens.Any(hrt => hrt.Token == refreshToken), ct);
    }

    public async Task<bool> IsBidderAsync(UserId userId, CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId && (u.Roles & UserRole.Bidder) == UserRole.Bidder, ct);
    }

    public Task<bool> IsEmailInUseAsync(Email email, CancellationToken ct)
    {
        return _context.Users.AsNoTracking().AnyAsync(u => u.Email.Equals(email), ct);
    }

    public async Task<bool> IsUserSellerAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId && (u.Roles & UserRole.Seller) == UserRole.Seller);
    }

public async Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<UserId> ids, CancellationToken ct = default)
    {
        if (ids == null || !ids.Any()) return new List<User>().AsReadOnly();

        return (await _context.Set<User>()
                .Where(u => ids.Contains(u.Id))
                .ToListAsync(ct)).AsReadOnly();
    }
}