using MazadZone.Application.Services;
using MazadZone.Domain.Payments.Errors;
using MazadZone.Domain.Repositories;
using MzadZone.Domain.Payments;
using MzadZone.Domain.Payments.Entities;

namespace MazadZone.Application.Features.Payments.Commands.CaptureAuthorizedHold;

/// <summary>
/// Handles the command to capture an authorized hold for a payment. This involves interacting with the payment service to capture the hold amount and updating the payment record accordingly. Logs are generated for each step of the process, including attempts, successes, and failures. The handler ensures that the payment state is consistent with the outcome of the capture operation.
/// </summary> 
/// <param name="request">The command containing the payment and gateway hold ID to capture.</param>
/// <param name="ct">Cancellation token for the operation.</param>
/// returns>A result indicating success or failure of the capture operation.</returns>
public class CaptureAuthorizedHoldCommandHandler : ICommandHandler<CaptureAuthorizedHoldCommand, Unit>
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<CaptureAuthorizedHoldCommandHandler> _logger;

    public CaptureAuthorizedHoldCommandHandler(
        IPaymentService paymentService,
        IPaymentRepository paymentRepository,
        ILogger<CaptureAuthorizedHoldCommandHandler> logger)
    {
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(CaptureAuthorizedHoldCommand request, CancellationToken ct)
    {
        CaptureAuthorizedHoldLogs.LogAttempt(_logger, request.Payment.Id, request.GatewayAuthHoldId);

        var payment = request.Payment;

        // Release the hold on the bidder's credit card and capture it as a charge
        var captureTransactionId = await _paymentService.CaptureHoldedAmountAsync(request.GatewayAuthHoldId, ct);

        if (string.IsNullOrEmpty(captureTransactionId))
        {
            CaptureAuthorizedHoldLogs.LogCaptureFailed(_logger, payment.Id, request.GatewayAuthHoldId);
            return PaymentErrors.DpositedAmountCaptureFailed;
        }

        // Record the capture transaction
        var result = payment.RecordTransactionAttempt(
            captureTransactionId,
            TransactionType.DepositCaptured
        );

        if (result.IsFailure)
        {
            CaptureAuthorizedHoldLogs.LogFailure(_logger, payment.Id, result.TopError.Message);
            return result.TopError;
        }

        // Mark the capture as successful
        var resolveResult = payment.ResolveTransactionSuccess(captureTransactionId);

        if (resolveResult.IsFailure)
        {
            payment.ResolveTransactionFailure(
                captureTransactionId,
                resolveResult.TopError.Message);

            CaptureAuthorizedHoldLogs.LogFailure(
                _logger,
                payment.Id,
                resolveResult.TopError.Message);

            return resolveResult.TopError;
        }

        _paymentRepository.Update(payment);

        CaptureAuthorizedHoldLogs.LogSuccess(_logger, payment.Id, captureTransactionId);

        return Unit.Value;
    }
}
