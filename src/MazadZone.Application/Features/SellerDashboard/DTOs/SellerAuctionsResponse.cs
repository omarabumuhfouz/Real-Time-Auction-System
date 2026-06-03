namespace MazadZone.Application.Features.SellerDashboard.DTOs;

public sealed record SellerAuctionSummaryDto
{
    public Guid AuctionId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int BidsCount { get; init; }
    public decimal LastBidAmount { get; init; }
    public DateTime EndDateUtc { get; init; }
    public string? ThumbnailUrl { get; init; }
}

public sealed record SellerAuctionsResponse
{
    public int ActiveAuctions { get; init; }
    public int Pending { get; init; }
    public int SoldItems { get; init; }
    public int Unsold { get; init; }
    public int TotalCount { get; init; }
    public IReadOnlyList<SellerAuctionSummaryDto> Auctions { get; init; } = Array.Empty<SellerAuctionSummaryDto>();
}
