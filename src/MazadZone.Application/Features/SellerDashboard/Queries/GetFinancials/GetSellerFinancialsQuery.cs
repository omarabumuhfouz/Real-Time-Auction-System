using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.SellerDashboard.DTOs;
using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Users.ValueObjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MazadZone.Application.Features.SellerDashboard.Queries.GetFinancials;

public sealed record GetSellerFinancialsQuery(UserId SellerId, SellerDashboardFilter Filter) : IQuery<SellerFinancialsResponse>;

public sealed class GetSellerFinancialsQueryHandler : IQueryHandler<GetSellerFinancialsQuery, SellerFinancialsResponse>
{
    private readonly ISellerDashboardQueries _queries;

    public GetSellerFinancialsQueryHandler(ISellerDashboardQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<SellerFinancialsResponse>> Handle(GetSellerFinancialsQuery request, CancellationToken cancellationToken)
    {
        var result = await _queries.GetSellerFinancialsAsync(request.SellerId, request.Filter, cancellationToken);
        return result ?? new SellerFinancialsResponse();
    }
}
