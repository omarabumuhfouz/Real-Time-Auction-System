using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.AddFeedback;
public sealed class NotifySellerOnFeedbackLeftDomainEventHandler : INotificationHandler<FeedbackLeftDomainEvent>
{
    private readonly INotificationRepository _notificationService;
    private readonly ISellerRepository _sellerRepository;

    public NotifySellerOnFeedbackLeftDomainEventHandler(
        ISellerRepository sellerRepository,
        INotificationRepository notificationService)
    {
        _sellerRepository = sellerRepository;
        _notificationService = notificationService;
    }

    public async Task Handle(FeedbackLeftDomainEvent notification, CancellationToken ct)
    {
        var seller = await _sellerRepository.GetByAuctionIdAsync(notification.AuctionId, ct);
        
        await _notificationService.NotifySellerAsync(
            seller.Id.Value, 
            "New Feedback Received",
            $"You received a {notification.Rating}-star review!", 
            ct);
    }
}