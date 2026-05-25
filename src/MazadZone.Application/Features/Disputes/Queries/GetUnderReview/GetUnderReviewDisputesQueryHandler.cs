
namespace MazadZone.Application.Features.Disputes.Queries.GetUnderReview;

public class GetUnderReviewDisputesQueryHandler : IQueryHandler<GetUnderReviewDisputesQuery, IReadOnlyList<DisputeDto>>
{
    private readonly IDisputeQueries _repository;
    public async Task<Result<IReadOnlyList<DisputeDto>>> Handle(GetUnderReviewDisputesQuery request, CancellationToken ct)
    {
        return Result.Success(await _repository.GetUnderReviewsAsync(ct) ?? new List<DisputeDto>());
    }
}