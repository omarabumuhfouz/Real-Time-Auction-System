using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;

public record GetBidderProfileQuery(UserId BidderId) : IQuery<BidderProfileDto>;