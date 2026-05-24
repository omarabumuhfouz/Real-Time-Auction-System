namespace MazadZone.Application.Features.Sellers.Queries.GetDashboard;

public sealed record SellerDashboardResponse
{
    public int ActiveAuctions { get; init; }
    public int SoldItems { get; init; }
    public int Pending { get; init; }
    public int Unsold { get; init; }

    public IReadOnlyList<SellerAuctionSummaryDto>? Auctions { get; init; }
}
