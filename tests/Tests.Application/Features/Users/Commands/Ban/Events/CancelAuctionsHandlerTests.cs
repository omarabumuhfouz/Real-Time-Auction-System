using MazadZone.Application.Features.Users.Commands.Ban.Events;
using MazadZone.Application.Features.Users.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;
using Tests.Application.Features.Auctions;

namespace Tests.Application.Features.Users.Commands.Ban;

public class CancelAuctionsHandlerTests : UserBaseTest<CancelAuctionsHandler>
{
    [Fact]
    public async Task Handle_UserIsNotSeller_SkipsAuctionCancellation()
    {
        // Arrange
        var userId = UserId.New();
        var domainEvent = new UserBannedDomainEvent(userId, "Violation", "test@test.com");

        // Setup: User is just a Bidder, not a Seller
        _userRepository.IsUserSellerAsync(userId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Verify the short-circuit worked and no auction queries were triggered
        await _auctionQueries.DidNotReceiveWithAnyArgs()
            .GetActiveAuctionsWithBiddersBySellerIdAsync(userId!, default);
            
        await _auctionRepository.DidNotReceiveWithAnyArgs()
            .TerminateAllAuctionsBySellerIdAsync(userId, default!, default);
    }

    [Fact]
    public async Task Handle_UserIsSeller_TerminatesAuctionsAndNotifiesBidders()
    {
        // Arrange
        var userId = UserId.New();
        var reason = "Fraudulent items detected.";
        var domainEvent = new UserBannedDomainEvent(userId, reason, "seller@test.com");

        var bidderOne = BidderId.New();
        var bidderTwo = BidderId.New();

        var mockActiveAuctions = AuctionHelper.CreateMockActiveAuctions(bidderOne, bidderTwo);

        _userRepository.IsUserSellerAsync(userId, Arg.Any<CancellationToken>())
            .Returns(true);

        _auctionQueries.GetActiveAuctionsWithBiddersBySellerIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(mockActiveAuctions);

        _auctionRepository.TerminateAllAuctionsBySellerIdAsync(userId, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(2); // 2 auctions terminated

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert 1: Verify Bulk Termination was called with the exact reason
        await _auctionRepository.Received(1).TerminateAllAuctionsBySellerIdAsync(
            userId,
            reason,
            Arg.Any<CancellationToken>());

        // Assert 2: Verify Bidder 1 received TWO notifications (one for each auction)
        await _notificationRepository.Received(2).NotifyBidderAsync(
            bidderOne.Value,
            Arg.Any<string>(),
            Arg.Any<string>());

        // Assert 3: Verify Bidder 2 received ONE notification (for the watch)
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
        var domainEvent = new UserBannedDomainEvent(userId, "Violation", "test@test.com");

        _userRepository.IsUserSellerAsync(userId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Return an empty list indicating no active auctions
        _auctionQueries.GetActiveAuctionsWithBiddersBySellerIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<AuctionBiddersDto>()); 

        _auctionRepository.TerminateAllAuctionsBySellerIdAsync(userId, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(0);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Ensure bulk terminate was still called (to sweep any edge cases), but no notifications were sent
        await _auctionRepository.Received(1).TerminateAllAuctionsBySellerIdAsync(
            userId,
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());

        await _notificationRepository.DidNotReceiveWithAnyArgs().NotifyBidderAsync(default!, default!, default!);
    }
}