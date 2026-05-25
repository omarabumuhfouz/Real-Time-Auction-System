namespace MazadZone.Application.Features.Sellers.Queries.GetDashboard;

public sealed record SellerDashboardFilter
{
    public string? Status { get; init; } // e.g., "Active", "Pending", "Ended", "Unsold"
    public string? SortBy { get; init; } // "EndDate", "LastBid", etc.
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
