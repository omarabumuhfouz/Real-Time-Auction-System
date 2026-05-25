using MazadZone.Application.Features.Disputes;
using MazadZone.Domain.Disputes;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.ResolveDispute;

public class ResolveDisputeCommandHandler : ICommandHandler<ResolveDisputeCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDisputeRepository _disputeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResolveDisputeCommandHandler> _logger;

    public ResolveDisputeCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<ResolveDisputeCommandHandler> logger,
        IDisputeRepository disputeRepository
        )
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _disputeRepository = disputeRepository;
    }

    public async Task<Result<Unit>> Handle(ResolveDisputeCommand request, CancellationToken ct)
    {
        ResolveDisputeLogs.LogAttempt(_logger, request.OrderId, request.Resolution);

        var order = await _orderRepository.GetWithDispute(request.OrderId, ct);

        if (order is null)
        {
            GlobalLogs.LogOrderNotFound(_logger, request.OrderId);
            return OrderErrors.NotFound;
        }

        var dispute = await _disputeRepository.GetByOrderIdAsync(request.OrderId, ct);

        if (dispute is null)
        {
            DisputeLogs.LogDisputeNotFound(_logger, request.OrderId);
            return DisputeErrors.NotFound;
        }

        var resolutionResult = Resolution.Create(request.Resolution);
        if (resolutionResult.IsFailure) return resolutionResult.TopError;

        var resolveDisputeResult = dispute.Resolve(resolutionResult.Value);
        
        if (resolveDisputeResult.IsFailure) 
        {
            ResolveDisputeLogs.LogDomainViolation(_logger, request.OrderId, resolveDisputeResult.TopError.Message);
            return resolveDisputeResult.TopError;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        ResolveDisputeLogs.LogSuccess(_logger, request.OrderId);

        return Unit.Value;
    }
}