namespace MazadZone.Application.Features.Auctions.DTOs;

public readonly record struct AuctionStatusUpdateDto
(Guid AuctionId,
 string Status);