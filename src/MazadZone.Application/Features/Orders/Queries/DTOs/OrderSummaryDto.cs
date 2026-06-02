namespace MazadZone.Application.Features.Orders.Queries.DTOs;

public record OrderSummaryDto(
    Guid OrderId,         
    string AuctionName,   
    string CategoryName,  
    string BidderName,    
    string BidderEmail,   
    string Status,           
    DateTime OrderDate,   
    decimal TotalAmount,  
    string Currency       
);
