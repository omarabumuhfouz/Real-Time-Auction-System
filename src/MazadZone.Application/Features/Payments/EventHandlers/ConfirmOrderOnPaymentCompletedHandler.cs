using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MazadZone.Domain.Payments.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Payments.EventHandlers;

public class ConfirmOrderOnPaymentCompletedHandler(
    IOrderRepository _orderRepository,
    IUnitOfWork _unitOfWork,
    ILogger<ConfirmOrderOnPaymentCompletedHandler> _logger
) : INotificationHandler<PaymentCompletedDomainEvent>
{
    public async Task Handle(PaymentCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling payment completion event for Order ID: {OrderId}", notification.OrderId.Value);

        //get order to change status 
        var order = await _orderRepository.GetByIdAsync(notification.OrderId, cancellationToken);
        
        if (order is null)
        {
            _logger.LogCritical("Financial Inconsistency: Payment verified for non-existent Order ID: {OrderId}", notification.OrderId.Value);
            return;
        }

        //change the status 
        var confirmationResult = order.Confirm();

        if (confirmationResult.IsFailure)
        {
            _logger.LogWarning("Order {OrderId} code rejected confirmation: {Error}", 
                order.Id.Value, confirmationResult.TopError.Message);
            return;
        }

        //save in DB
        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} state successfully transitioned to Confirmed.", order.Id.Value);
    }
}