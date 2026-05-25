namespace MazadZone.Application.Features.Disputes.Queries.GetById;

public record GetDisputeByIdQuery(DisputeId DisputeId) : IQuery<DisputeDetailsDto>;