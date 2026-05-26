using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Features.Disputes.Queries;

public interface IDisputeQueries : IScopedService
{
    Task<DisputeDetailsDto?> GetDetailsByIdAsync(DisputeId disputeId, CancellationToken ct);
    Task<IReadOnlyList<DisputeListItemDto>> GetFilteredDisputesAsync(DisputeFilterParams filters, CancellationToken ct);

}