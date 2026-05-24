using MazadZone.Application.Common.Interfaces;

namespace MazadZone.Application.Features.Sellers.Queries.GetDashboard;

public sealed class GetSellerDashboardQueryHandler : IQueryHandler<GetSellerDashboardQuery, SellerDashboardResponse>
{
    private readonly ISellerQueries _sellerQueries;

    public GetSellerDashboardQueryHandler(ISellerQueries sellerQueries)
    {
        _sellerQueries = sellerQueries;
    }

    public async Task<Result<SellerDashboardResponse>> Handle(GetSellerDashboardQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter ?? new SellerDashboardFilter();
        
        var result = await _sellerQueries.GetSellerDashboardAsync(request.SellerId, filter, cancellationToken);
        
        return result ?? new SellerDashboardResponse { Auctions = Array.Empty<SellerAuctionSummaryDto>() };
    }
}
