using MazadZone.Application.Features.Users.Commands.Suspend.Events;
using MazadZone.Application.Features.Users.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;
using Tests.Application.Features.Auctions;

namespace Tests.Application.Features.Users.Commands.Suspend.Events;

public class CancelAuctionsOnSuspensionHandlerTests : UserBaseTest<CancelAuctionsOnSuspensionHandler>
{
    [Fact]
    public async Task Handle_UserIsNotSeller_SkipsAuctionCancellation()
    {
        // Arrange
        var userId = UserId.New();
        var suspendUntil = DateTime.UtcNow.AddDays(7);

        var domainEvent = UserHelper.CreateSuspensionEvent(userId, suspendUntil);

        _userRepository.IsUserSellerAsync(userId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Verify the short-circuit worked and no queries/writes were triggered
        await _auctionQueries.DidNotReceiveWithAnyArgs()
            .GetActiveAuctionsWithBiddersBySellerIdAsync(userId, default);
            
        await _auctionRepository.DidNotReceiveWithAnyArgs()
            .TerminateAllAuctionsBySellerIdAsync(userId, default!, default);
    }

    [Fact]
    public async Task Handle_UserIsSeller_TerminatesAuctionsAndNotifiesBidders()
    {
        // Arrange
        var userId = UserId.New();
        var suspendUntil = DateTime.UtcNow.AddDays(14);
        var domainEvent = UserHelper.CreateSuspensionEvent(userId, suspendUntil);

        var bidderOne = BidderId.New();
        var bidderTwo = BidderId.New();

        var mockActiveAuctions = AuctionHelper.CreateMockActiveAuctions(bidderOne, bidderTwo);

        // Setup 
        _userRepository.IsUserSellerAsync(userId, Arg.Any<CancellationToken>())
            .Returns(true);

        _auctionQueries.GetActiveAuctionsWithBiddersBySellerIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(mockActiveAuctions);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert 1: Verify Bulk Termination was called with the exact reason
        await _auctionRepository.Received(1).TerminateAllAuctionsBySellerIdAsync(
            userId,
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());

        // Assert 2: Verify Bidder 1 received a notification
        await _notificationRepository.Received(2).NotifyBidderAsync(
            bidderOne.Value,
            Arg.Any<string>(),
            Arg.Any<string>());


        await _notificationRepository.Received(1).NotifyBidderAsync(
            bidderTwo.Value,
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_SellerHasNoActiveAuctions_TerminatesAuctionsAndSkipsNotifications()
    {
        // Arrange
        var userId = UserId.New();
        var suspendUntil = DateTime.UtcNow.AddDays(3);
        var domainEvent = UserHelper.CreateSuspensionEvent(userId, suspendUntil);

        _userRepository.IsUserSellerAsync(userId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Return an empty list indicating the seller has no active auctions
        _auctionQueries.GetActiveAuctionsWithBiddersBySellerIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<AuctionBiddersDto>());

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Ensure bulk terminate was still called, but no notifications were sent
        await _auctionRepository.Received(1).TerminateAllAuctionsBySellerIdAsync(
            userId,
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());

        await _notificationRepository.DidNotReceiveWithAnyArgs().NotifyBidderAsync(default!, default!, default!);
    }
}
