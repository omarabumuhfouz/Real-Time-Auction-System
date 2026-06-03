using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.SellerDashboard.DTOs;
using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Users.ValueObjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System;

namespace MazadZone.Application.Features.SellerDashboard.Queries.GetAuctions;

public sealed record GetSellerAuctionsQuery(UserId SellerId, SellerDashboardFilter Filter) : IQuery<SellerAuctionsResponse>;

public sealed class GetSellerAuctionsQueryHandler : IQueryHandler<GetSellerAuctionsQuery, SellerAuctionsResponse>
{
    private readonly ISellerDashboardQueries _queries;

    public GetSellerAuctionsQueryHandler(ISellerDashboardQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<SellerAuctionsResponse>> Handle(GetSellerAuctionsQuery request, CancellationToken cancellationToken)
    {
        var result = await _queries.GetSellerAuctionsAsync(request.SellerId, request.Filter, cancellationToken);
        return result ?? new SellerAuctionsResponse { Auctions = Array.Empty<SellerAuctionSummaryDto>() };
    }
}
