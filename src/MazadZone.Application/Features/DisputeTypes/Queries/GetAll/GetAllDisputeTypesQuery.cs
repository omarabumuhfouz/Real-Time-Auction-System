namespace MazadZone.Features.DisputeTypes.Queries.GetAll;

public record GetAllDisputeTypesQuery() : IQuery<IReadOnlyList<DisputeTypeDto>>;