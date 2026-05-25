namespace MazadZone.Features.DisputeTypes.Commands.Update;

using Microsoft.Extensions.Logging;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Disputes;

internal sealed class UpdateDisputeTypeCommandHandler : ICommandHandler<UpdateDisputeTypeCommand, Unit>
{
    private readonly IDisputeTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateDisputeTypeCommandHandler> _logger;

    public UpdateDisputeTypeCommandHandler(
        IDisputeTypeRepository repository, 
        IUnitOfWork unitOfWork, 
        ILogger<UpdateDisputeTypeCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UpdateDisputeTypeCommand request, CancellationToken ct)
    {
        var disputeType = await _repository.GetByIdAsync(request.DisputeTypeId, ct);
        if (disputeType is null)
        {
            DisputeTypeLogs.LogNotFound(_logger, request.DisputeTypeId);
            return DisputeTypeErrors.NotFound;
        }

        var nameResult = Name.Create(request.Name);
        if (nameResult.IsFailure)
        {
            DisputeTypeLogs.LogDomainViolation(_logger, nameResult.TopError.Code, nameResult.TopError.Message);
            return nameResult.TopError;
        }

        var descriptionResult = Description.Create(request.Description);
        if (descriptionResult.IsFailure)
        {
            DisputeTypeLogs.LogDomainViolation(_logger, descriptionResult.TopError.Code, descriptionResult.TopError.Message);
            return descriptionResult.TopError;
        }

        disputeType.Update(nameResult.Value, descriptionResult.Value);

        await _unitOfWork.SaveChangesAsync(ct);

        DisputeTypeLogs.LogUpdateSuccess(_logger, request.DisputeTypeId);
        return Unit.Value;
    }
}