using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Disputes.Queries.GetOpenDisputesBreakdown;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Features.Disputes.Queries;

public interface IDisputeQueries : IScopedService
{
    Task<DisputeDetailsDto?> GetDetailsByIdAsync(DisputeId disputeId, CancellationToken ct);
    Task<PagedList<DisputeListItemDto>> GetFilteredDisputesAsync(DisputeFilterParams filters, CancellationToken ct);
    Task<IReadOnlyList<RawDisputeBreakdown>> GetOpenDisputesBreakdownAsync(
        DateTime currStart, DateTime currEnd,
        DateTime prevStart, DateTime prevEnd,
        CancellationToken ct);

    Task<IReadOnlyList<DisputeListItemDto>> ExportSelectedDisputesAsync(IEnumerable<Guid> disputeIds, CancellationToken ct);


}