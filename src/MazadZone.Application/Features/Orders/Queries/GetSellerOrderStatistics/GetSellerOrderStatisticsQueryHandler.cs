using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Orders.Queries.GetSellerOrderStatistics;

public class GetSellerOrderStatisticsQueryHandler : IQueryHandler<GetSellerOrderStatisticsQuery, OrderStatisticsDto>
{
    private readonly IOrderQueries _orderQueries;

    public GetSellerOrderStatisticsQueryHandler(IOrderQueries orderQueries)
    {
        _orderQueries = orderQueries;
    }

    public async Task<Result<OrderStatisticsDto>> Handle(GetSellerOrderStatisticsQuery request, CancellationToken ct)
    {
        var stats = await _orderQueries.GetSellerOrderStatisticsAsync(request.SellerId, ct);
        
        return stats;
    }
}