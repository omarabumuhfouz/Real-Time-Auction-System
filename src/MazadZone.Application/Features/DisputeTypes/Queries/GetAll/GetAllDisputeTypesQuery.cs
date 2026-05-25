namespace MazadZone.Features.DisputeTypes.Queries.GetAll;

public record DisputeTypeDto(Guid Id, string Name, string Description, bool IsActive);

public record GetAllDisputeTypesQuery() : IQuery<IReadOnlyList<DisputeTypeDto>>;