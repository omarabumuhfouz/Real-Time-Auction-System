using MazadZone.Domain.Disputes;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Disputes.Commands.OpenDispute;

public class OpenDisputeCommandHandler : ICommandHandler<OpenDisputeCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDisputeRepository _disputeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OpenDisputeCommandHandler> _logger;

    public OpenDisputeCommandHandler(
        IOrderRepository orderRepository, 
        IUnitOfWork unitOfWork,
        ILogger<OpenDisputeCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(OpenDisputeCommand request, CancellationToken ct)
    {
        // OpenDisputeLogs.LogAttempt(_logger, request.OrderId, request.Reason);

        var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);

        if (order is null)
        {
            GlobalLogs.LogOrderNotFound(_logger, request.OrderId);
            return OrderErrors.NotFound;
        }

        if (!order.IsDisputable) return OrderErrors.CannotDispute;

        var dispute = await _disputeRepository.GetByOrderIdAsync(request.OrderId, ct);

        if (dispute is null)
        {
            return DisputeErrors.NotFound;
            //Log here
        }

        var titleResult = Title.Create(request.Title);
        if (titleResult.IsFailure) return titleResult.TopError;

        var descriptionResult = Description.Create(request.Description);
        if (descriptionResult.IsFailure) return descriptionResult.TopError;


        var disputeResult = dispute.Open(request.OrderId, request.DisputeTypeId, titleResult.Value, descriptionResult.Value, request.Images);

        if (disputeResult.IsFailure)
        {
            OpenDisputeLogs.LogDomainViolation(_logger, request.OrderId, disputeResult.TopError.Message);
            return disputeResult.TopError;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        OpenDisputeLogs.LogSuccess(_logger, request.OrderId);

        return Unit.Value;
    }

}