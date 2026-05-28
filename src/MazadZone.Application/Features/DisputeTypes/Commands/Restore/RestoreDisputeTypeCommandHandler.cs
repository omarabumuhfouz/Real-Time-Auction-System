namespace MazadZone.Features.DisputeTypes.Commands.Restore;

using Microsoft.Extensions.Logging;
using MazadZone.Domain.Disputes;

public sealed class RestoreDisputeTypeCommandHandler : ICommandHandler<RestoreDisputeTypeCommand, Unit>
{
    private readonly IDisputeTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreDisputeTypeCommandHandler> _logger;

    public RestoreDisputeTypeCommandHandler(
        IDisputeTypeRepository repository, 
        IUnitOfWork unitOfWork, 
        ILogger<RestoreDisputeTypeCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RestoreDisputeTypeCommand request, CancellationToken ct)
    {
        var disputeType = await _repository.GetByIdAsync(request.DisputeTypeId, ct);
        if (disputeType is null)
        {
            DisputeTypeLogs.LogNotFound(_logger, request.DisputeTypeId);
            return DisputeTypeErrors.NotFound;
        }

        if (disputeType.IsActive) return Unit.Value;

        var result = disputeType.Restore();
        if (result.IsFailure)
        {
            DisputeTypeLogs.LogDomainViolation(_logger, result.TopError.Code, result.TopError.Message);
            return result.TopError;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        DisputeTypeLogs.LogRestoreSuccess(_logger, request.DisputeTypeId);
        return Unit.Value;
    }
}