using MazadZone.Application.Features.Payments.Commands.UnauthorizeOutbidPayments;
using MazadZone.Domain.Auctions.Events;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Auctions.Commands.EndAuction.EventHandlers;



public class ReleaseLosingBidsEventHandler(
    IAuctionRepository _auctionRepository,
    ISender _sender,
    ILogger<ReleaseLosingBidsEventHandler> _logger
) : INotificationHandler<AuctionEndedDomainEvent>
{
    public async Task Handle(AuctionEndedDomainEvent notification, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(notification.AuctionId, cancellationToken);
        if (auction is null) return;

        var winningBidId = auction.CurrentLeadingBid?.Id;

        // losing Bids Id holded
        var losingHoldIds = auction.Bids
            .Where(b => b.Id != winningBidId && !string.IsNullOrEmpty(b.GatewayAuthHoldId))
            .Select(b => b.GatewayAuthHoldId)
            .ToList();

        if (losingHoldIds.Any())
        {
            //logic
            var result = await _sender.Send(new UnauthorizeOutbidPaymentsCommand(losingHoldIds), cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to release some holds: {Error}", result.TopError.Message);
            }
        }
    }
}

