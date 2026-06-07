namespace MazadZone.Application.Features.SellerDashboard.DTOs;

public sealed record SellerOrderSummaryDto
{
    public Guid OrderId { get; init; }
    public Guid AuctionId { get; init; }
    public string AuctionTitle { get; init; } = string.Empty;
    public string OrderStatus { get; init; } = string.Empty;
    public DateTime OrderDateUtc { get; init; }
    public decimal TotalAmount { get; init; }
    public string BidderName { get; init; } = string.Empty;
}

public sealed record SellerOrdersResponse
{
    public int TotalCount { get; init; }
    public IReadOnlyList<SellerOrderSummaryDto> Orders { get; init; } = Array.Empty<SellerOrderSummaryDto>();
}
