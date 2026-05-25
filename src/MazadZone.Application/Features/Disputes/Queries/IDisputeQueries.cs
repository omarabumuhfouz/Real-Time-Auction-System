using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Features.Disputes.Queries;

public interface IDisputeQueries : IScopedService
{
    Task<DisputeDetailsDto?> GetByIdAsync(DisputeId disputeId, CancellationToken ct);
    Task<IReadOnlyList<DisputeDto>?> GetOpensAsync(CancellationToken ct);
    Task<IReadOnlyList<DisputeDto>?> GetResolvedAsync(CancellationToken ct);
    Task<IReadOnlyList<DisputeDto>?> GetUnderReviewsAsync(CancellationToken ct);


}