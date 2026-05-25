using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Features.DisputeTypes.Queries.GetAll;
using MazadZone.Features.DisputeTypes.Queries.GetById;

public interface IDisputeTypeQueries : IScopedService
{
    Task<IReadOnlyList<DisputeTypeDto>> GetAllAsync(CancellationToken ct);
    Task<DisputeTypeResponse> GetByIdAsync(DisputeTypeId id, CancellationToken ct);



}