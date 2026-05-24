using MazadZone.Application.Services;
using MazadZone.Domain.Repositories;
using MzadZone.Domain.Payments.Entities;

namespace MazadZone.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentService _paymentService;


    public CancelOrderCommandHandler(
        IOrderRepository orderRepository, 
        IUnitOfWork unitOfWork,
        IPaymentRepository paymentRepository,
        IPaymentService paymentService,
        ILogger<CancelOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _paymentRepository = paymentRepository;
        _paymentService = paymentService;
    }

    public async Task<Result<Unit>> Handle(CancelOrderCommand request, CancellationToken ct)
    {

        CancelOrderLogs.LogAttempt(_logger, request.OrderId);

        var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);

        if (order is null) 
        {
            GlobalLogs.LogOrderNotFound(_logger, request.OrderId);
            return OrderErrors.NotFound;
        }

        var cancellationResult = order.Cancel();
        
        if (cancellationResult.IsFailure) 
        {
            CancelOrderLogs.LogDomainViolation(_logger, request.OrderId, cancellationResult.TopError.Message);
            return cancellationResult.TopError;
        }

        //capture the auth amount
        var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId.Value, ct);
        if (payment != null)
        {
            var authHold = payment.Transactions.FirstOrDefault(t => 
                t.Type == TransactionType.AuthorizationHold && 
                t.Status == TransactionStatus.Success);

            if (authHold != null)
            {
                _logger.LogInformation("Capturing 10% penalty for unpaid order: {OrderId}", request.OrderId);
                
                payment.RecordTransactionAttempt(authHold.GatewayIntentId, TransactionType.DepositCaptured);
                
                var captureId = await _paymentService.CaptureHoldedAmountAsync(authHold.GatewayIntentId, ct);
                
                if (string.IsNullOrEmpty(captureId))
                {
                    payment.ResolveTransactionFailure(authHold.GatewayIntentId, "Penalty capture failed.");
                    _logger.LogError("Failed to capture penalty for order: {OrderId}", request.OrderId);                }
                else
                {
                    payment.ResolveTransactionSuccess(captureId);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        CancelOrderLogs.LogSuccess(_logger, request.OrderId);

        return Unit.Value;
    }
}