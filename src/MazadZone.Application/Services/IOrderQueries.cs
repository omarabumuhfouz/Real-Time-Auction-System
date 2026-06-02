using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Application.Features.Orders.Queries.GetSellerOrderStatistics;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;
using MazadZone.Domain.Shared.Interfaces;
using MzadZone.Domain.Payments;

namespace MazadZone.Application.Services;

public interface IOrderQueries : IScopedService
{
    // --- Detailed View ---
    Task<OrderDetailsDto?> GetOrderDetailsAsync(OrderId orderId, CancellationToken ct = default);
    Task<OrderStatisticsDto> GetSellerOrderStatisticsAsync(UserId sellerId, CancellationToken ct);
    
    // --- List & Search (Refactored GetOrderHistory) ---
    Task<PagedList<OrderSummaryDto>> SearchOrdersAsync(OrderSearchFilter filter, CancellationToken ct = default);

    // --- Specialized Lookups ---
    Task<OrderDetailsDto?> GetOrderByWinningBidAsync(BidId winningBidId, CancellationToken ct = default);

    Task<AdminGlobalStatsDto> GetGlobalStatsAsync(CancellationToken ct = default);
    Task<PagedList<OrderSummaryDto>> GetSellerOrdersTableAsync(
        UserId sellerId,
        OrderStatus? statusFilter,
        int page,
        int pageSize,
        CancellationToken ct);

    // --- Payment Queries ---
    Task<Payment?> GetPaymentByOrderIdAsync(OrderId orderId, CancellationToken ct = default);

    Task<AuctionId> GetAuctionIdByOrderIdAsync(OrderId orderId, CancellationToken ct = default);
}