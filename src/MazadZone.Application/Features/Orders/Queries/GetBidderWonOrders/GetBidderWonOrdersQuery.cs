using MazadZone.Application.Common.Paging;

namespace MazadZone.Application.Features.Orders.Queries.GetBidderWonOrders;

public record GetBidderWonOrdersQuery(
    UserId BidderId, 
    string? Status, // String representation from the UI (e.g., "Pending", "Confirmed", "All")
    int Page, 
    int PageSize
) : IQuery<PagedList<WonOrderSummaryDto>>;