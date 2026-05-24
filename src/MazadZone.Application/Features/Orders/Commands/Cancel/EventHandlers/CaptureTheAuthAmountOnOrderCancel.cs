using MazadZone.Application.Services;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;
using MzadZone.Domain.Payments.Entities;

namespace MazadZone.Application.Features.Orders.Commands.Cancel.EventHandlers;


public class CaptureTheAuthAmountOnCancelOrder
(
    IPaymentRepository _paymentRepository,
    IPaymentService _paymentService,
    ILogger<CaptureTheAuthAmountOnCancelOrder> _logger
    
) : INotificationHandler<OrderCancelledDomainEvent>
{
    public async Task Handle(OrderCancelledDomainEvent notification, CancellationToken ct)
    {
                //capture the auth amount
        var payment = await _paymentRepository.GetByOrderIdAsync(notification.OrderId.Value, ct);
        if (payment != null)
        {
            var authHold = payment.Transactions.FirstOrDefault(t => 
                t.Type == TransactionType.AuthorizationHold && 
                t.Status == TransactionStatus.Success);

            if (authHold != null)
            {
                _logger.LogInformation("Capturing 10% penalty for unpaid order: {OrderId}", notification.OrderId);
                
                payment.RecordTransactionAttempt(authHold.GatewayIntentId, TransactionType.DepositCaptured);
                
                var captureId = await _paymentService.CaptureHoldedAmountAsync(authHold.GatewayIntentId, ct);
                
                if (string.IsNullOrEmpty(captureId))
                {
                    payment.ResolveTransactionFailure(authHold.GatewayIntentId, "Penalty capture failed.");
                    _logger.LogError("Failed to capture penalty for order: {OrderId}", notification.OrderId);                }
                else
                {
                    payment.ResolveTransactionSuccess(captureId);
                }
            }
        }
    }
}