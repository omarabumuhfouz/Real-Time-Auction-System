namespace MazadZone.Application.Features.Disputes.Queries.GetResolved;

public record GetResolvedDisputesQuery() : IQuery<IReadOnlyList<DisputeListItemDto>>;