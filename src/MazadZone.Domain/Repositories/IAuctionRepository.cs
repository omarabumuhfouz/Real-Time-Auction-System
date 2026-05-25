using MazadZone.Domain.Auctions;
using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Repositories;

public interface IAuctionRepository : IGenericRepository<Auction, AuctionId>, IScopedService
{
    Task<int> TerminateAllAuctionsBySellerIdAsync(UserId sellerId, string reason, CancellationToken ct);
    Task<int> CancelAllActiveBySellerIdAsync(Guid sellerId, CancellationToken ct);
    Task<int> RemoveActiveBidsByBidderIdAsync(UserId bidderId, CancellationToken ct);
    Task<Auction?> GetByIdWithBidsAsync(AuctionId id, CancellationToken cancellationToken = default);
}