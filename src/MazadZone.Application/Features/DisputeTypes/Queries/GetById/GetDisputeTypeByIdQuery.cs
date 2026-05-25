namespace MazadZone.Features.DisputeTypes.Queries.GetById;

public record DisputeTypeResponse(Guid Id, string Name, string Description, bool IsActive);

public record GetDisputeTypeByIdQuery(DisputeTypeId DisputeTypeId) : IQuery<DisputeTypeResponse>;