using MazadZone.Domain.Payments.Events;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Notifications;
using MazadZone.Application.Services;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Notifications.Enums;

namespace MazadZone.Application.Features.Payments.EventHandlers;

public class PaymentCompletedNotificationHandler(
    IPaymentRepository _paymentRepository,
    INotificationRepository _notificationRepository,
    IRealTimeNotificationService _realTimeService,
    IUnitOfWork _unitOfWork,
    ILogger<PaymentCompletedNotificationHandler> _logger
) : INotificationHandler<PaymentCompletedDomainEvent>
{
    public async Task Handle(PaymentCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(notification.PaymentId, cancellationToken);
        
        if (payment is null)
        {
            _logger.LogError("Critical Data Inconsistency: Payment with ID {PaymentId} not found during notification processing.", notification.PaymentId.Value);
            return;
        }

        string title = "Payment Successful";
        string message = $"Your payment for order {notification.OrderId.Value} has been completed successfully. The order is now confirmed.";

        var dbNotification = Notification.Create(
            userId: payment.UserId,
            title: title,
            message: message
        );

        _notificationRepository.Add(dbNotification);
        
        // (SignalR)
        await _realTimeService.SendNotificationAsync(new UserNotificationDto(
            payment.UserId.Value,
            NotificationMethods.ReceiveNotification,
            title,
            message
        ), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}