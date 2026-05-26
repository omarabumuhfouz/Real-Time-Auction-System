using MazadZone.Domain.Auctions;
using MazadZone.Domain.Auctions.ValueObjects;
using MazadZone.Domain.Repositories;
using MazadZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Repositories;

public class ItemRepository(AppDbContext context) : IItemRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Item?> GetItemByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Items
            .Include(i => i.Images)
            .FirstOrDefaultAsync(i => i.Id == ItemId.From(id), ct);
    }

    public Task<IEnumerable<Item>> ListItemsBySellerIdAsync(Guid sellerId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}