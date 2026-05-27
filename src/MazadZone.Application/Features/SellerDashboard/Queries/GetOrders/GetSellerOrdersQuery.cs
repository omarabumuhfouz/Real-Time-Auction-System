using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.SellerDashboard.DTOs;
using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Users.ValueObjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System;

namespace MazadZone.Application.Features.SellerDashboard.Queries.GetOrders;

public sealed record GetSellerOrdersQuery(UserId SellerId, SellerDashboardFilter Filter) : IQuery<SellerOrdersResponse>;

public sealed class GetSellerOrdersQueryHandler : IQueryHandler<GetSellerOrdersQuery, SellerOrdersResponse>
{
    private readonly ISellerDashboardQueries _queries;

    public GetSellerOrdersQueryHandler(ISellerDashboardQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<SellerOrdersResponse>> Handle(GetSellerOrdersQuery request, CancellationToken cancellationToken)
    {
        var result = await _queries.GetSellerOrdersAsync(request.SellerId, request.Filter, cancellationToken);
        return result ?? new SellerOrdersResponse { Orders = Array.Empty<SellerOrderSummaryDto>() };
    }
}
