namespace MazadZone.Features.DisputeTypes.Commands.Create;

using Microsoft.Extensions.Logging;
using MazadZone.Domain.Disputes;

public sealed class CreateDisputeTypeCommandHandler : ICommandHandler<CreateDisputeTypeCommand, Guid>
{
    private readonly IDisputeTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDisputeTypeCommandHandler> _logger;

    public CreateDisputeTypeCommandHandler(
        IDisputeTypeRepository repository, 
        IUnitOfWork unitOfWork, 
        ILogger<CreateDisputeTypeCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateDisputeTypeCommand request, CancellationToken ct)
    {
        var result = DisputeType.Create(request.Name, request.Description);
        
        if (result.IsFailure)
        {
            DisputeTypeLogs.LogDomainViolation(_logger, result.TopError.Code, result.TopError.Message);
            return result.TopError;
        }

        _repository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(ct);
        
        DisputeTypeLogs.LogCreateSuccess(_logger, result.Value.Id);
        return result.Value.Id.Value;
    }
}