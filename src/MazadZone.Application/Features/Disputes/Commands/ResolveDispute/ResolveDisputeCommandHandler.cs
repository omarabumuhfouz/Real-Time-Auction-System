using MazadZone.Application.Features.Disputes;
using MazadZone.Domain.Disputes;

namespace MazadZone.Application.Features.Orders.Commands.ResolveDispute;

public class ResolveDisputeCommandHandler : ICommandHandler<ResolveDisputeCommand, Unit>
{
    private readonly IDisputeRepository _disputeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResolveDisputeCommandHandler> _logger;

    public ResolveDisputeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ResolveDisputeCommandHandler> logger,
        IDisputeRepository disputeRepository
        )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _disputeRepository = disputeRepository;
    }

    public async Task<Result<Unit>> Handle(ResolveDisputeCommand request, CancellationToken ct)
    {
        ResolveDisputeLogs.LogAttempt(_logger, request.DisputeId, request.Resolution);

        var dispute = await _disputeRepository.GetByIdAsync(request.DisputeId, ct);

        if (dispute is null)
        {
            DisputeLogs.LogDisputeNotFound(_logger, request.DisputeId);
            return DisputeErrors.NotFound;
        }

        var resolutionResult = Resolution.Create(request.Resolution);
        if (resolutionResult.IsFailure) return resolutionResult.TopError;

        var resolveDisputeResult = dispute.Resolve(resolutionResult.Value);
        
        if (resolveDisputeResult.IsFailure) 
        {
            ResolveDisputeLogs.LogDomainViolation(_logger, resolutionResult.TopError.Code, resolveDisputeResult.TopError.Message);
            return resolveDisputeResult.TopError;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        ResolveDisputeLogs.LogSuccess(_logger, request.DisputeId);

        return Unit.Value;
    }
}