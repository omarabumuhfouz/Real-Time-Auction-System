using MazadZone.Application.Features.Payments.Commands.UnauthorizeOutbidPayments;
using MazadZone.Domain.Repositories;
using MzadZone.Domain.Payments;
using MzadZone.Domain.Payments.Entities;

namespace MazadZone.Application.Features.Payments.Commands.RecordAuthorizationHold;


/// <summary>/ Handles the command to record an authorization hold for a payment. This involves updating the payment record with
/// the details of the authorization hold transaction, including the gateway hold ID. The handler logs each step of the process, including attempts, successes, and failures. It ensures that the payment state is consistent with the outcome of recording the authorization hold.
/// </summary>
/// <param name="request">The command containing the payment and gateway hold ID to record.</param>
/// <param name="ct">Cancellation token for the operation.</param>
/// returns>A result indicating success or failure of the record operation.</returns>
public class RecordAuthorizationHoldCommandHandler : ICommandHandler<RecordAuthorizationHoldCommand, Unit>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<RecordAuthorizationHoldCommandHandler> _logger;

    private readonly ISender _sender;   
    public RecordAuthorizationHoldCommandHandler(
        IPaymentRepository paymentRepository,
        ILogger<RecordAuthorizationHoldCommandHandler> logger,
        ISender sender)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
        _sender = sender;
    }

    public async Task<Result<Unit>> Handle(RecordAuthorizationHoldCommand request, CancellationToken ct)
    {
        RecordAuthorizationHoldLogs.LogAttempt(_logger, request.Payment.Id, request.GatewayAuthHoldId);

        var payment = request.Payment;

        // Record the authorization hold transaction
        var result = payment.RecordTransactionAttempt(
            gatewayIntentId: request.GatewayAuthHoldId,
            transactionType: TransactionType.AuthorizationHold
        );

        if (result.IsFailure)
        {
            RecordAuthorizationHoldLogs.LogFailure(_logger, payment.Id, request.GatewayAuthHoldId, result.TopError.Message);
            return result.TopError;
        }

        // Immediately mark it as successful, because this hold was already validated during the bidding phase
        var resolveResult = payment.ResolveTransactionSuccess(request.GatewayAuthHoldId);

        if (resolveResult.IsFailure)
        {
            payment.ResolveTransactionFailure(
                request.GatewayAuthHoldId,
                resolveResult.TopError.Message);
            
            // If resolving the transaction as successful fails, we should unauthorize the hold to release the bidder's funds
            var UnAuthorizeResult = await _sender.Send(new UnauthorizeOutbidPaymentsCommand(new List<string> { request.GatewayAuthHoldId }), ct);
           
            if (UnAuthorizeResult.IsFailure)
            {
                RecordAuthorizationHoldLogs.LogFailure(_logger, payment.Id, request.GatewayAuthHoldId, UnAuthorizeResult.TopError.Message);
            }
            else
            {
                RecordAuthorizationHoldLogs.LogSuccess(_logger, payment.Id, request.GatewayAuthHoldId);
            }
            RecordAuthorizationHoldLogs.LogFailure(
                _logger,
                payment.Id,
                request.GatewayAuthHoldId,
                resolveResult.TopError.Message);

            return resolveResult.TopError;
        }

        _paymentRepository.Update(payment);

        RecordAuthorizationHoldLogs.LogSuccess(_logger, payment.Id, request.GatewayAuthHoldId);

        return Unit.Value;
    }
}
