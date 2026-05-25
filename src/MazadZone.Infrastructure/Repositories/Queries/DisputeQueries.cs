namespace  MazadZone.Infrastructure.Repositories.Queries;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MazadZone.Application.Features.Disputes.Queries;
using MazadZone.Domain.Orders;

public class DisputeQueries : IDisputeQueries
{
    public Task<DisputeDetailsDto?> GetByIdAsync(DisputeId disputeId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<DisputeDto>?> GetOpensAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<DisputeDto>?> GetResolvedAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<DisputeDto>?> GetUnderReviewsAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}