using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Features.Disputes.Queries;

public interface IDisputeQueries : IScopedService
{
    Task<DisputeDetailsDto?> GetDetailsByIdAsync(DisputeId disputeId, CancellationToken ct);
    Task<IReadOnlyList<DisputeListItemDto>?> GetOpensAsync(CancellationToken ct);
    Task<IReadOnlyList<DisputeListItemDto>?> GetResolvedAsync(CancellationToken ct);
    Task<IReadOnlyList<DisputeListItemDto>?> GetUnderReviewsAsync(CancellationToken ct);


}