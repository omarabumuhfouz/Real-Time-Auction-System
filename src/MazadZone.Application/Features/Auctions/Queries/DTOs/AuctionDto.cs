using MazadZone.Application.Features.Auctions.Queries.Sellers;
using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.DTOs;

public record AuctionDto(
    Guid Id,
    string ItemTitle,
    string ItemDescription,
    IReadOnlyList<string> ImageUrls,
    string Seller,
    decimal StartBid,
    decimal MinBidAmount,
    string Currency,
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan RemainderTime, // EndTime - DateTime.UtcNow
    AuctionStatus AuctionStatus,
    bool IsActive,
    IReadOnlyList<BidDto> Bids
    
    );

