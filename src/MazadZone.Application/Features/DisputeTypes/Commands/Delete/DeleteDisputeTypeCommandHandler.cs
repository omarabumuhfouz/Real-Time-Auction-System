namespace MazadZone.Features.DisputeTypes.Commands.Delete;

using Microsoft.Extensions.Logging;
using MazadZone.Domain.Disputes;

internal sealed class DeleteDisputeTypeCommandHandler : ICommandHandler<DeleteDisputeTypeCommand, Unit>
{
    private readonly IDisputeTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteDisputeTypeCommandHandler> _logger;

    public DeleteDisputeTypeCommandHandler(
        IDisputeTypeRepository repository, 
        IUnitOfWork unitOfWork, 
        ILogger<DeleteDisputeTypeCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteDisputeTypeCommand request, CancellationToken ct)
    {
        var disputeType = await _repository.GetByIdAsync(request.DisputeTypeId, ct);
        if (disputeType is null)
        {
            DisputeTypeLogs.LogNotFound(_logger, request.DisputeTypeId);
            return DisputeTypeErrors.NotFound;
        }

        var result = disputeType.Delete();
        if (result.IsFailure)
        {
            DisputeTypeLogs.LogDomainViolation(_logger, result.TopError.Code, result.TopError.Message);
            return result.TopError;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        DisputeTypeLogs.LogDeleteSuccess(_logger, request.DisputeTypeId);
        return Unit.Value;
    }
}