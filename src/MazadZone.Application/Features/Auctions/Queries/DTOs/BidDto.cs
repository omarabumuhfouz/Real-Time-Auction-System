namespace MazadZone.Application.Features.Auctions.DTOs;

public sealed record BidDto(
    Guid Id,
    Guid AuctionId,
    Guid BidderId,
    decimal Amount,
    DateTime Timestamp
);