using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions.Events;
using MazadZone.Domain.Notifications;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Auctions.Commands.PlaceBid.EventHandlers;

/// <summary>
/// Handles the event when a bidder is outbid.
/// </summary>
/// <remarks>
/// This event handler listens for the BidderOutbidDomainEvent, which is raised whenever a bidder's bid is surpassed by another bidder's higher bid on an auction.
/// When a bidder is outbid, this handler retrieves the auction details and identifies the outbid bidder. It then sends a notification to the outbid bidder, informing them that their bid has been surpassed and encouraging them to place a new bid if they wish to remain competitive in the auction.
/// Additionally, it also triggers the payment service to un-authorize the hold on the outbid bidder's credit card for the previous bid amount, allowing them to have their funds available again for future bidding or other purchases.
/// This ensures a seamless and responsive user experience, keeping bidders informed in real-time about the status of their bids and allowing them to react quickly to changes in the auction dynamics.
/// </remarks>
/// <typeparam name="OutbidDomainEventHandler"></typeparam>

public class OutbidDomainEventHandler(
    ISender _sender
) : INotificationHandler<BidderOutbidDomainEvent>
{

    public async Task Handle(BidderOutbidDomainEvent notification, CancellationToken cancellationToken)
    {
        // Fetch the auction details to get the item information and current highest bid

        var NotificationId = await _sender.Send(new CreateNotificationCommand(
            UserId: UserId.Load(notification.OutbidBidderId.Value),
            NotificationMethods.ReceiveNotification,
            Title: $"Your bid of {notification.OutBidAmount} has been surpassed",
            Message: $"Your bid of {notification.OutBidAmount} has been surpassed. Place a new bid to stay in the game!"
        ), cancellationToken);


    }
}