using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Services;
public interface IBidderQueries
{
    Task<BidderProfileDto> GetBidderProfile(BidderId bidderId);
}