namespace MazadZone.Features.DisputeTypes.Queries.GetAll;

public sealed class GetAllDisputeTypesQueryHandler(IDisputeTypeQueries repository) : IQueryHandler<GetAllDisputeTypesQuery, IReadOnlyList<DisputeTypeDto>>
{
    public async Task<Result<IReadOnlyList<DisputeTypeDto>>> Handle(GetAllDisputeTypesQuery request, CancellationToken ct)
    {
        return Result.Success(await repository.GetAllAsync(ct) ?? new List<DisputeTypeDto>());
    }
}