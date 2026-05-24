
using MazadZone.Domain.Repositories;
using MazadZone.Application.Services;
using MzadZone.Domain.Payments.Entities;
using MazadZone.Domain.Payments.Errors;
using MazadZone.Domain.Payments.Enums;

namespace MazadZone.Application.Features.Payments.Commands.PayRemainingAmount;

public class PayRemainingAmountCommandHandler(
    IPaymentRepository _paymentRepository,
    IPaymentService _paymentService,
    IPaymentQueries _paymentQueries,
    IUnitOfWork _unitOfWork,
    ILogger<PayRemainingAmountCommandHandler> _logger
) : IRequestHandler<PayRemainingAmountCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(PayRemainingAmountCommand request, CancellationToken cancellationToken)
    {
        // 1. get the payment for order
        var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure<Unit>(PaymentErrors.NotFound);
        }
        // must been authorized firsly
        if (payment.Status != PaymentStatus.Authorized)
        {
            return Result.Failure<Unit>(PaymentErrors.MissingAuthorizationHold);
        }

        // 2. calculate the remaning amount 
        Money totalAmount = await _paymentQueries.GetTotalAmountByOrderIdAsync(payment.OrderId.Value, cancellationToken);
        Money depositAmount = totalAmount * 0.10m;
        Money remainingAmount = totalAmount - depositAmount;

        //3. get the authId for old transaction (autherize amount 10%)
        var originalAuthHold = payment.Transactions
            .FirstOrDefault(t => t.Type == TransactionType.AuthorizationHold && t.Status == TransactionStatus.Success);
        
        if (originalAuthHold is null)
        {
            return Result.Failure<Unit>(PaymentErrors.MissingAuthorizationHold);
        }

        // 4. Cupture the 90% remaning amount
        _logger.LogInformation("Attempting to charge remaining 90% ({Amount}) for Order {OrderId}", remainingAmount, request.OrderId);
        
        // record try pay 90% to help admin tracing Pending payments if provider faliure 
        string remainingChargeIntentId = $"int_rem_{Guid.NewGuid()}"; // temprory
        payment.RecordTransactionAttempt(remainingChargeIntentId, TransactionType.RemainingAmountCapture);

        var gatewayTransactionId = await _paymentService.ChargeAmountAsync(
            payment.UserId.Value, 
            remainingAmount, 
            request.PaymentMethodId, 
            cancellationToken);

        if (string.IsNullOrEmpty(gatewayTransactionId))
        {
            // record failure in transaction table
            payment.ResolveTransactionFailure(remainingChargeIntentId, "Card declined or insufficient funds.");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<Unit>(PaymentErrors.RemainingAmountCaptureFailed);
        }

        // capture 90% is sucess
        payment.ResolveTransactionSuccess(remainingChargeIntentId); 

        // 5. capture the 10% to get 100% amount
        _logger.LogInformation("Remaining 90% paid. Capturing original 10% hold: {HoldId}", originalAuthHold.GatewayIntentId);
        
        // record try capture 
        payment.RecordTransactionAttempt(originalAuthHold.GatewayIntentId, TransactionType.DepositCaptured);

        var captureResultId = await _paymentService.CaptureHoldedAmountAsync(originalAuthHold.GatewayIntentId, cancellationToken);

        if (string.IsNullOrEmpty(captureResultId))
        {
            // failure deposit 10%
            _logger.LogCritical("Fatal Financial Mismatch: Charged 90% but failed to capture 10% hold for Payment {PaymentId}", payment.Id);
            payment.ResolveTransactionFailure(originalAuthHold.GatewayIntentId, "Failed to capture original hold after charging remaining balance.");
            
            // save the current status to enter the support from bank
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<Unit>(PaymentErrors.CriticalPaymentMismatch);
        }

        // capture the 10% is sucess
        payment.ResolveTransactionSuccess(originalAuthHold.GatewayIntentId);

        // 6. save the payment transactions in database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}