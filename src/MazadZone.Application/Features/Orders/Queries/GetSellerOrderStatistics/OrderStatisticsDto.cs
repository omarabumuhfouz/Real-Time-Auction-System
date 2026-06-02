namespace MazadZone.Application.Features.Orders.Queries.GetSellerOrderStatistics;

public record OrderStatisticsDto(
    int TotalOrders,
    int PendingCount,
    int ConfirmedCount, 
    int ShippedCount,
    int DeliveredCount,
    int CanceledCount
);