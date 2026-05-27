namespace MazadZone.Application.Features.SellerDashboard.DTOs;

public sealed record SellerDashboardFilter
{
    public string? Status { get; init; }
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; } // Date, Price, Bids
    public string? SortDirection { get; init; } // Asc, Desc
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
