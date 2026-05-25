
namespace MazadZone.Application.Features.Disputes.Queries.GetOpens;

public class GetOpenDisputesQueryHandler : IQueryHandler<GetOpenDisputesQuery, IReadOnlyList<DisputeDto>>
{
    private readonly IDisputeQueries _repository;

    public GetOpenDisputesQueryHandler(IDisputeQueries repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DisputeDto>>> Handle(GetOpenDisputesQuery request, CancellationToken ct)
    {
        return Result.Success(await _repository.GetOpensAsync(ct) ?? new List<DisputeDto>());
    }
}