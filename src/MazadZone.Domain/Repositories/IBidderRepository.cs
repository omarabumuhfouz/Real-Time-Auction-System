using MazadZone.Domain.Bidders;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Domain.Repositories;
public interface IBidderRepository : IGenericRepository<Bidder, BidderId> , IScopedService
{
    Task<string?> GetNationalIdByBidderIdAsync(BidderId bidderId, CancellationToken cancellationToken);

}