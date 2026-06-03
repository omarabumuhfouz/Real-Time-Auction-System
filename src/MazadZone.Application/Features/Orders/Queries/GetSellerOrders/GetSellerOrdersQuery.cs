using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;

namespace MazadZone.Application.Features.Orders.Queries.GetSellerOrders;

public record GetSellerOrdersQuery(
    UserId SellerId, 
    string? Status, // Receives the raw string from the frontend (e.g. "Pending", "All")
    int Page, 
    int PageSize
) : IQuery<PagedList<OrderSummaryDto>>;