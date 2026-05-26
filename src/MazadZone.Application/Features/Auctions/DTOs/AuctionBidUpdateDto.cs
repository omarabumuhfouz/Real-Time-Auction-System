namespace MazadZone.Application.Features.Auctions.DTOs;

public readonly record struct AuctionBidUpdateDto
(Guid AuctionId, 
decimal NewPrice);