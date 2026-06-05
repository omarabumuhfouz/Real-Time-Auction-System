using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Application.Features.Notifications.Enums;
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Auctions.Events;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Auctions.Commands.EndAuction.EventHandlers;

public class NotifyWinnerOnEndedAuctionEventHandler(
    IAuctionRepository _auctionRepository,
    IItemRepository _itemRepository,
    ISender _sender,
    ILogger<NotifyWinnerOnEndedAuctionEventHandler> _logger
) : INotificationHandler<AuctionEndedDomainEvent>
{
    public async Task Handle(AuctionEndedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling winner notification for ended auction: {AuctionId}", notification.AuctionId);

        var auction = await _auctionRepository.GetByIdWithBidsAsync(notification.AuctionId, cancellationToken);
        if (auction is null)
        {
            _logger.LogWarning("Auction with ID {AuctionId} not found.", notification.AuctionId);
            return;
        }

        var winningBid = auction.CurrentLeadingBid;
        if (winningBid is null)
        {
            _logger.LogInformation("Auction with ID {AuctionId} ended with no bids.", notification.AuctionId);
            return;
        }

        var item = await _itemRepository.GetItemByIdAsync(auction.Item.Id.Value, cancellationToken);
        var itemTitle = item?.Title ?? "your auction item";

        var title = $"Congratulations! You won the auction for {itemTitle}";
        var message = $"You won the auction for {itemTitle} with a bid of {winningBid.Amount.Amount:C}. Please complete your payment.";

        var notificationId = await _sender.Send(new CreateNotificationCommand(
            UserId: UserId.Load(winningBid.BidderId.Value),
            NotificationMethods.ReceiveNotification,
            Title: title,
            Message: message
        ), cancellationToken);

        _logger.LogInformation("Winner notification sent successfully to bidder {BidderId}. NotificationId: {NotificationId}", winningBid.BidderId, notificationId);
    }
}
