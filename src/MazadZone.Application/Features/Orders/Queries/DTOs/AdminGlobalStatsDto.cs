namespace MazadZone.Application.Features.Orders.Queries.DTOs;

public record AdminGlobalStatsDto(
    decimal TotalSalesVolume,      
    int TotalOrders,               
    
    decimal TotalRealizedRevenue,  
    decimal AverageOrderValue,     
    
    decimal TotalPendingAmount,    // Sum of Pending orders
    int TotalPendingOrders,        // Count of Pending orders
    
    decimal TotalCanceledAmount,   // Sum of Canceled orders
    int TotalCanceledOrders,       // Count of Canceled orders
    
    int TotalActiveDisputes        // Count of unresolved disputes
)
{
    public static AdminGlobalStatsDto Empty => new(0, 0, 0, 0, 0, 0, 0, 0, 0);
}