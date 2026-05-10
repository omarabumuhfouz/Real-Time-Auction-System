using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.OpenDispute;

/// <summary>
/// Alerts the administrative team to a new dispute that may require mediation.
/// </summary>
public sealed class AlertAdminOnDisputeOpenedDomainEventHandler 
    : INotificationHandler<DisputeOpenedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public AlertAdminOnDisputeOpenedDomainEventHandler(
        INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(DisputeOpenedDomainEvent notification, CancellationToken ct)
    {
        // This usually goes to a central 'Admins' dashboard or shared support email
        const string title = "New Mediation Required: Dispute #{0}";
        var message = $@"A new dispute has been opened for Order #{notification.OrderId.Value}. 
        Dispute ID: {notification.DisputeId.Value}
        Action: Please review the dispute details and prepare for potential mediation between the bidder and seller.";

        await _notificationRepository.NotifyAdminAsync(
            title: string.Format(title, notification.DisputeId.Value),
            message: message,
            ct: ct);
    }
}