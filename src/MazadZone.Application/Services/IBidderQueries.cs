using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Bidders;

namespace MazadZone.Application.Services;
public interface IBidderQueries
{
    Task<BidderProfileDto?> GetBidderProfile(BidderId bidderId, CancellationToken cancellationToken = default);
}