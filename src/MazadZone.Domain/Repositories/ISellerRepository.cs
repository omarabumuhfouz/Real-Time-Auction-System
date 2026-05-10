using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Domain.Repositories;

public interface ISellerRepository : IGenericRepository<Seller>, IScopedService
{
    public Task<Seller?> GetByAuctionIdAsync(AuctionId auctionId, CancellationToken ct);

}
