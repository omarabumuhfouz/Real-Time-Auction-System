namespace MazadZone.Application.Features.SellerDashboard.DTOs;

public sealed record SellerFinancialsResponse
{
    public decimal TotalGrossRevenue { get; init; }
    public decimal TotalPlatformFees { get; init; }
    public decimal TotalNetProfit { get; init; }
    public int CompletedOrdersCount { get; init; }
}
