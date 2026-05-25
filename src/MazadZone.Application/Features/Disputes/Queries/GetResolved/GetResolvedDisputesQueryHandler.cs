
namespace MazadZone.Application.Features.Disputes.Queries.GetResolved;

public class GetResolvedDisputesQueryHandler : IQueryHandler<GetResolvedDisputesQuery, IReadOnlyList<DisputeListItemDto>>
{
    private readonly IDisputeQueries _repository;

    public GetResolvedDisputesQueryHandler(IDisputeQueries repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DisputeListItemDto>>> Handle(GetResolvedDisputesQuery request, CancellationToken ct)
    {
        return Result.Success(await _repository.GetResolvedAsync(ct) ?? new List<DisputeListItemDto>());
    }
}