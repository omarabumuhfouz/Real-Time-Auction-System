using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;
using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Repositories;

public interface ISellerRepository : IGenericRepository<Seller, UserId>, IScopedService
{
    public Task<Seller?> GetByAuctionIdAsync(AuctionId auctionId, CancellationToken ct);
    public Task<Seller?> GetByOrderIdAsync(OrderId orderId, CancellationToken ct);

}
