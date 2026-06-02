using MazadZone.Domain.Bidders;
using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Domain.Repositories;
public interface IBidderRepository : IGenericRepository<Bidder, UserId> , IScopedService
{
    Task<string?> GetNationalIdByBidderIdAsync(UserId bidderId, CancellationToken cancellationToken);

    Task<bool> IsNationalIdInUseAsync(string nationalId, CancellationToken cancellationToken);
}