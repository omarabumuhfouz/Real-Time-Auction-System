namespace MazadZone.Application.Features.Disputes.Queries.GetUnderReview;

public record GetUnderReviewDisputesQuery() : IQuery<IReadOnlyList<DisputeDto>>;