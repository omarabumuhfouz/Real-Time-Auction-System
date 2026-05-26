using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Services;
public interface IBidderQueries : IScopedService
{
    Task<BidderProfileDto?> GetBidderProfile(UserId bidderId, CancellationToken cancellationToken = default);
}