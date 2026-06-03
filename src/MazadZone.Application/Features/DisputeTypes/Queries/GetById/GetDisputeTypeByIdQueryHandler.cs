using MazadZone.Domain.Disputes;
using MazadZone.Features.DisputeTypes.Queries.GetAll;

namespace MazadZone.Features.DisputeTypes.Queries.GetById;


public sealed class GetDisputeTypeByIdQueryHandler : IQueryHandler<GetDisputeTypeByIdQuery, DisputeTypeDto>
{
    private readonly IDisputeTypeQueries _repository;
    private readonly ILogger<GetDisputeTypeByIdQueryHandler> _logger;

    public GetDisputeTypeByIdQueryHandler(IDisputeTypeQueries repository, ILogger<GetDisputeTypeByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<DisputeTypeDto>> Handle(GetDisputeTypeByIdQuery request, CancellationToken ct)
    {
        var disputeType = await _repository.GetByIdAsync(request.DisputeTypeId, ct);
        if (disputeType is null)
        {
            DisputeTypeLogs.LogNotFound(_logger, request.DisputeTypeId);
            return DisputeTypeErrors.NotFound;
        }

        return disputeType;
    }
}