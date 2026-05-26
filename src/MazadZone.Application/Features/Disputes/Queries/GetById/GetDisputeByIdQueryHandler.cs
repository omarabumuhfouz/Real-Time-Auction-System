namespace MazadZone.Application.Features.Disputes.Queries.GetById;

using MazadZone.Domain.Disputes;

public class GetDisputeByIdQueryHandler : IQueryHandler<GetDisputeByIdQuery, DisputeDetailsDto>
{
    private readonly IDisputeQueries _repository;
    private readonly ILogger<GetDisputeByIdQueryHandler> _logger;

    public GetDisputeByIdQueryHandler(IDisputeQueries repository, ILogger<GetDisputeByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<DisputeDetailsDto>> Handle(GetDisputeByIdQuery request, CancellationToken ct)
    {
        var disputeDto = await _repository.GetDetailsByIdAsync(request.DisputeId, ct);
        if (disputeDto is null)
        {
            DisputeLogs.LogDisputeNotFound(_logger, request.DisputeId);
            return DisputeErrors.NotFound;
        }

        return disputeDto;
    }
}