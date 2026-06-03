using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Orders.Queries.GetSellerOrders;

public class GetSellerOrdersQueryHandler : IQueryHandler<GetSellerOrdersQuery, PagedList<OrderSummaryDto>>
{
    private readonly IOrderQueries _orderQueries;

    public GetSellerOrdersQueryHandler(IOrderQueries orderQueries)
    {
        _orderQueries = orderQueries;
    }

    public async Task<Result<PagedList<OrderSummaryDto>>> Handle(GetSellerOrdersQuery request, CancellationToken ct)
    {
        // 1. Safely parse the string to the Enum
        OrderStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && !request.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            parsedStatus = Enum.Parse<OrderStatus>(request.Status, ignoreCase: true);
        }

        // 2. Fetch the paginated data from Dapper
        var pagedResult = await _orderQueries.GetSellerOrdersTableAsync(
            request.SellerId, 
            parsedStatus, 
            request.Page, 
            request.PageSize, 
            ct);

        return Result.Success(pagedResult);
    }
}