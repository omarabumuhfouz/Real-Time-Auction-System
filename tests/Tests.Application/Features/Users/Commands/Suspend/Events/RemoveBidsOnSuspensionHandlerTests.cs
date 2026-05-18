using MazadZone.Application.Features.Users.Commands.Ban.Models;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;
using Tests.Application.Features.Auctions;

namespace Tests.Application.Features.Users.Commands.Suspend.Events;

public class RemoveBidsOnSuspensionHandlerTests : UserBaseTest<RemoveBidsOnSuspensionHandler>
{
    [Fact]
    public async Task Handle_UserIsNotBidder_SkipsBidRemoval()
    {
        // Arrange
        var userId = UserId.New();
        var suspendUntil = DateTime.UtcNow.AddDays(7);
        var domainEvent = UserHelper.CreateSuspensionEvent(userId, suspendUntil);

        // Setup: User might be an Admin or purely a Seller
        _userRepository.IsBidderAsync(userId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - Verify the short-circuit worked to save DB calls
        await _auctionQueries.DidNotReceiveWithAnyArgs()
            .GetAuctionsByBidderIdAsync(userId, default);
            
        await _auctionRepository.DidNotReceiveWithAnyArgs()
            .RemoveActiveBidsByBidderIdAsync(userId, default);
    }

    [Fact]
    public async Task Handle_BidderHasNoActiveBids_SkipsBidRemoval()
    {
        // Arrange
        var userId = UserId.New();
        var suspendUntil = DateTime.UtcNow.AddDays(7);
        var domainEvent = UserHelper.CreateSuspensionEvent(userId, suspendUntil);

        _userRepository.IsBidderAsync(userId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Return an empty list to simulate a bidder with no current market activity
        _auctionQueries.GetAuctionsByBidderIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<AffectedAuctionDto>()); 

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert - The query was executed, but deletion and notifications were skipped
        await _auctionQueries.ReceivedWithAnyArgs(1)
            .GetAuctionsByBidderIdAsync(userId, default);

        await _auctionRepository.DidNotReceiveWithAnyArgs()
            .RemoveActiveBidsByBidderIdAsync(userId, default);
            
        await _notificationRepository.DidNotReceiveWithAnyArgs()
            .NotifySellerAsync(default!, default!, default!);
    }

    [Fact]
    public async Task Handle_BidderHasActiveBids_RemovesBidsAndNotifiesStakeholders()
    {
        // Arrange
        var suspendedUserId = UserId.New();
        var suspendUntil = DateTime.UtcNow.AddDays(3);
        var domainEvent = UserHelper.CreateSuspensionEvent(suspendedUserId, suspendUntil);

        var sellerOneId = SellerId.New();
        var sellerTwoId = SellerId.New();
        var innocentBidderId = BidderId.New();

        var mockAffectedAuctions = AuctionHelper.CreateMockAffectedAuctions(sellerOneId, sellerTwoId, innocentBidderId);

        // Setup - Vogen Safe Matchers
        _userRepository.IsBidderAsync(suspendedUserId, Arg.Any<CancellationToken>())
            .Returns(true);

        _auctionQueries.GetAuctionsByBidderIdAsync(suspendedUserId, Arg.Any<CancellationToken>())
            .Returns(mockAffectedAuctions);

        _auctionRepository.RemoveActiveBidsByBidderIdAsync(suspendedUserId, Arg.Any<CancellationToken>())
            .Returns(2); // Simulated 2 bids removed 

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert 1: Verify Bulk Deletion was triggered for the suspended user
        await _auctionRepository.Received(1).RemoveActiveBidsByBidderIdAsync(
            suspendedUserId,
            Arg.Any<CancellationToken>());

        // Assert 2: Verify Seller 1 was notified about the Monitor
        await _notificationRepository.Received(1).NotifySellerAsync(
            sellerOneId.Value,
            Arg.Any<string>(),
            Arg.Any<string>());

        // Assert 3: Verify Seller 2 was notified about the Keyboard
        await _notificationRepository.Received(1).NotifySellerAsync(
            sellerTwoId.Value,
            Arg.Any<string>(),
            Arg.Any<string>());

        // Assert 4: Verify the Innocent Bidder was notified about the price change on the Monitor
        await _notificationRepository.Received(1).NotifyBidderAsync(
            innocentBidderId.Value,
            Arg.Any<string>(),
            Arg.Any<string>());
    }
}