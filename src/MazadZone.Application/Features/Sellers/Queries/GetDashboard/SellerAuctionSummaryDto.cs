namespace MazadZone.Application.Features.Sellers.Queries.GetDashboard;

public sealed record SellerAuctionSummaryDto
{
    public Guid AuctionId { get; init; }
    public string? Title { get; init; }
    public string? Category { get; init; }
    public string? Status { get; init; }
    public int BidsCount { get; init; }
    public decimal? LastBidAmount { get; init; }
    public DateTime? EndDateUtc { get; init; }
    public string? ThumbnailUrl { get; init; }
}
