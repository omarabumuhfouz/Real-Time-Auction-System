
namespace MazadZone.Application.Features.Disputes.Queries.GetUnderReview;

public class GetUnderReviewDisputesQueryHandler : IQueryHandler<GetUnderReviewDisputesQuery, IReadOnlyList<DisputeListItemDto>>
{
    private readonly IDisputeQueries _repository;

    public GetUnderReviewDisputesQueryHandler(IDisputeQueries repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DisputeListItemDto>>> Handle(GetUnderReviewDisputesQuery request, CancellationToken ct)
    {
        return Result.Success(await _repository.GetUnderReviewsAsync(ct) ?? new List<DisputeListItemDto>());
    }
}