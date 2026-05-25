using MazadZone.Features.DisputeTypes.Queries.GetAll;
using MazadZone.Features.DisputeTypes.Queries.GetById;

namespace MazadZone.Infrastructure.Repositories.Queries;

public class DisputeTypeQueries : IDisputeTypeQueries
{
    public Task<IReadOnlyList<DisputeTypeDto>> GetAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<DisputeTypeResponse> GetByIdAsync(DisputeTypeId id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}