using MazadZone.Domain.Auctions.Events;
using MazadZone.Domain.Repositories;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Auctions.Commands.PlaceBid.EventHandlers
{
    public class ReleaseAuthToOutBidEventHandler(
        IAuctionRepository auctionRepository,
        IPaymentService paymentService,
        ILogger<ReleaseAuthToOutBidEventHandler> logger
    ) : INotificationHandler<BidderOutbidDomainEvent>
    {
        public async Task Handle(BidderOutbidDomainEvent notification, CancellationToken cancellationToken)
        {
            var auction = await auctionRepository.GetByIdAsync(notification.AuctionId, cancellationToken);
            if (auction is null)
            {
                logger.LogWarning("Auction {AuctionId} not found when trying to release outbid hold", notification.AuctionId);
                return;
            }

            var outbidBid = auction.Bids.FirstOrDefault(b => b.Id == notification.OutbidBidId);
            if (outbidBid is null)
            {
                logger.LogWarning("Outbid bid {BidId} not found in auction {AuctionId}", notification.OutbidBidId, notification.AuctionId);
                return;
            }

            if (string.IsNullOrEmpty(outbidBid.GatewayAuthHoldId))
            {
                logger.LogInformation("No gateway auth hold ID associated with outbid bid {BidId}", notification.OutbidBidId);
                return;
            }

            logger.LogInformation("Releasing hold {HoldId} for outbid bidder {BidderId}", outbidBid.GatewayAuthHoldId, notification.OutbidBidderId);

            try
            {
                await paymentService.UnAuthorizeAsync(outbidBid.GatewayAuthHoldId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to release authorization hold {HoldId} for outbid bid {BidId}", outbidBid.GatewayAuthHoldId, notification.OutbidBidId);
            }
        }
    }
}