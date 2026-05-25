namespace MazadZone.Application.Features.Orders.Commands.Deliver.Events;

using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class RecordCompletedPurchaseOnOrderDelivery : INotificationHandler<OrderDeliveredDomainEvent>
{
    private readonly IBidderRepository _bidderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RecordCompletedPurchaseOnOrderDelivery> _logger;

    public RecordCompletedPurchaseOnOrderDelivery(IBidderRepository bidderRepository, IUnitOfWork unitOfWork, ILogger<RecordCompletedPurchaseOnOrderDelivery> logger)
    {
        _bidderRepository = bidderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(OrderDeliveredDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing purchase point allocation for Order: {OrderId}", notification.OrderId);

        var bidder = await _bidderRepository.GetByIdAsync(notification.BidderId, cancellationToken);

        if (bidder is null)
        {
            _logger.LogWarning("Profile stats wrapper not found for purchasing User: {UserId}.", notification.BidderId);
            return;
        }

        // Domain method call remains clean and matching
        bidder.RecordCompletePurchase();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}