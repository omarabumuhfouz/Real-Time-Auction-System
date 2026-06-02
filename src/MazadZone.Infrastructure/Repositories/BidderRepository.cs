using MazadZone.Domain.Bidders;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Repositories;

public class BidderRepository : GenericRepository<Bidder, UserId>, IBidderRepository
{
    private readonly AppDbContext _context;

    public BidderRepository(AppDbContext context) : base(context)
    {
        _context = context;
        
    }

    public async Task<string?> GetNationalIdByBidderIdAsync(UserId bidderId, CancellationToken ct)
    {
        return await _context.Bidders
             .AsNoTracking()
             .Where(b => b.Id == bidderId)
             .Select(b => b.NationalId)
             .FirstOrDefaultAsync();
    }

    public async Task<bool> IsNationalIdInUseAsync(string nationalId, CancellationToken cancellationToken)
    {
        return await _context.Bidders
            .AsNoTracking()
            .AnyAsync(b => b.NationalId == nationalId, cancellationToken);  
    }
}