using MazadZone.Domain.Repositories;
using MazadZone.Domain.Orders.Events; 

namespace MazadZone.Application.Features.Orders.Events.NotifyBidderOnFeedbackReplied;

public class NotifyBidderOnFeedbackRepliedEventHandler 
    : INotificationHandler<FeedbackRepliedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotifyBidderOnFeedbackRepliedEventHandler> _logger;

    public NotifyBidderOnFeedbackRepliedEventHandler(
        INotificationRepository notificationRepository,
        ILogger<NotifyBidderOnFeedbackRepliedEventHandler> logger
    )
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task Handle(FeedbackRepliedDomainEvent notification, CancellationToken ct)
    {
        await _notificationRepository.NotifyBidderAsync(
            bidderId: notification.BidderId.Value,
            title: "Seller Replied to Your Feedback",
            message: $"The seller left a response to your feedback on Order #{notification.OrderId.Value}.",
            ct: ct
        );
    }
}
