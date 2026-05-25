
namespace MazadZone.Application.Features.Disputes.Queries.GetResolved;

public class GetResolvedDisputesQueryHandler : IQueryHandler<GetResolvedDisputesQuery, IReadOnlyList<DisputeDto>>
{
    private readonly IDisputeQueries _repository;

    public GetResolvedDisputesQueryHandler(IDisputeQueries repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DisputeDto>>> Handle(GetResolvedDisputesQuery request, CancellationToken ct)
    {
        return Result.Success(await _repository.GetResolvedAsync(ct) ?? new List<DisputeDto>());
    }
}