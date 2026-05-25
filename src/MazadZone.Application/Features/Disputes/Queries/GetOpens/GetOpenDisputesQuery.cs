namespace MazadZone.Application.Features.Disputes.Queries.GetOpens;

public record GetOpenDisputesQuery() : IQuery<IReadOnlyList<DisputeDto>>;