using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Features.DisputeTypes.Queries.GetAll;

public interface IDisputeTypeQueries : IScopedService
{
    Task<IReadOnlyList<DisputeTypeDto>?> GetAllAsync(CancellationToken ct);
    Task<DisputeTypeDto?> GetByIdAsync(DisputeTypeId id, CancellationToken ct);
}