using MazadZone.Application.Common.Paging;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Orders.Queries.GetBidderWonOrders;

public class GetBidderWonOrdersQueryHandler : IQueryHandler<GetBidderWonOrdersQuery, PagedList<WonOrderSummaryDto>>
{
    private readonly IOrderQueries _orderQueries;

    public GetBidderWonOrdersQueryHandler(IOrderQueries orderQueries)
    {
        _orderQueries = orderQueries;
    }

    public async Task<Result<PagedList<WonOrderSummaryDto>>> Handle(GetBidderWonOrdersQuery request, CancellationToken ct)
    {
        // 1. Safely parse the string to the Enum
        OrderStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && !request.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            parsedStatus = Enum.Parse<OrderStatus>(request.Status, ignoreCase: true);
        }

        // 2. Fetch the paginated data
        var pagedResult = await _orderQueries.GetBidderWonOrdersAsync(
            request.BidderId, 
            parsedStatus, 
            request.Page, 
            request.PageSize, 
            ct);

        return Result.Success(pagedResult);
    }
}