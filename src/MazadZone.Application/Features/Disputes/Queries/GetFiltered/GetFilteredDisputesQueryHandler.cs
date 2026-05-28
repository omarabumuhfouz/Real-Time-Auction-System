namespace MazadZone.Application.Features.Disputes.Queries.GetFiltered;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MazadZone.Application.Common.Paging;

public class GetFilteredDisputesQueryHandler : IQueryHandler<GetFilteredDisputesQuery, PagedList<DisputeListItemDto>>
{
    private readonly IDisputeQueries _repository;

    public GetFilteredDisputesQueryHandler(IDisputeQueries repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedList<DisputeListItemDto>>> Handle(GetFilteredDisputesQuery request, CancellationToken ct)
    {
        // Map the MediatR query to the Dapper filter params we created earlier
        var filterParams = new DisputeFilterParams
        {
            SearchTerm = request.SearchTerm,
            Status = request.Status,
            CategoryId = request.CategoryId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            SortColumn = request.SortColumn ?? "SubmittedDate", // Default fallback
            IsDescending = request.IsDescending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var disputes = await _repository.GetFilteredDisputesAsync(filterParams, ct);
        
        return Result.Success(disputes ?? PagedList<DisputeListItemDto>.Empty());
    }
}