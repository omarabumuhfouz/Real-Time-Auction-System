using MazadZone.Domain.Repositories;
using MazadZone.Domain.Orders.Events;
using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;

namespace MazadZone.Application.Features.Orders.Events.NotifyBidderOnFeedbackReplied;

public class NotifyBidderOnFeedbackRepliedEventHandler 
    : INotificationHandler<FeedbackRepliedDomainEvent>
{
    private readonly ISender _sender;

    public NotifyBidderOnFeedbackRepliedEventHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task Handle(FeedbackRepliedDomainEvent notification, CancellationToken ct)
    {
        var title = "Seller Replied to Your Feedback";
        var message = $"The seller left a response to your feedback on Order #{notification.OrderId.Value}.";
        
        await _sender.Send(new CreateNotificationCommand(
                                    notification.BidderId,
                                    NotificationMethods.ReceiveNotification,
                                    title,
                                    message
                            ));
    }
}
