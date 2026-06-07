namespace MazadZone.Application.Features.Orders.Queries.GetBidderWonOrders;

public record WonOrderSummaryDto(
    Guid OrderId,
    string ItemTitle,
    decimal FinalBidAmount, // Maps to Order Total Amount
    DateTime OrderDate,
    Guid SellerId,
    string SellerName,
    string Status // Returned as a string for easy UI rendering
);