namespace MazadZone.Application.Features.Orders.EventHandlers;

using MediatR;
using MazadZone.Domain.Orders.Events;
using Microsoft.Extensions.Logging;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Auctions;

public sealed class IncrementAuctionsWonOnOrderCreated : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IBidderRepository _bidderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IncrementAuctionsWonOnOrderCreated> _logger;

    public IncrementAuctionsWonOnOrderCreated(
        IBidderRepository bidderRepository,
        IUnitOfWork unitOfWork,
        ILogger<IncrementAuctionsWonOnOrderCreated> logger
    )
    {
        _bidderRepository = bidderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing win metrics calculation for Buyer: {BuyerId} from Order: {OrderId}", 
            notification.BidderId, 
            notification.OrderId);

        // Fetch the profile aggregate tracking entity using the Buyer's UserId
        var bidder = await _bidderRepository.GetByIdAsync(notification.BidderId, cancellationToken);

        if (bidder is null)
        {
            _logger.LogWarning(
                "Profile metrics aggregate not found for winning user: {BuyerId}. Analytics increment bypassed.", 
                notification.BidderId);
            return;
        }

        // Execute encapsulated domain mutation logic safely 
        bidder.RecordAuctionWon();

        // Persist the updated state to the data store
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully incremented AuctionsWon for User: {BuyerId}.", 
            notification.BidderId);
    }
}