using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;

namespace MazadZone.Application.Features.Sellers.Queries.GetFeedbacks;
public class GetSellerFeedbacksQueryHandler : IQueryHandler<GetSellerFeedbacksQuery, PagedList<FeedbackDto>>
{
    private readonly ISellerQueries _sellerQueries;
    public GetSellerFeedbacksQueryHandler(ISellerQueries sellerQueries) => _sellerQueries = sellerQueries;

    public async Task<Result<PagedList<FeedbackDto>>> Handle(GetSellerFeedbacksQuery request, CancellationToken ct)
    {
        var pagedFeedbacks = await _sellerQueries.GetSellerFeedbacksAsync(request.SellerId, request.Page, request.PageSize, ct);
        return Result.Success(pagedFeedbacks);
    }
}