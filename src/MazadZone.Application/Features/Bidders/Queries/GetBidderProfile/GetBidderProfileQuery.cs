using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Bidders;

namespace MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;

public record GetBidderProfileQuery(BidderId BidderId) : IQuery<BidderProfileDto>;