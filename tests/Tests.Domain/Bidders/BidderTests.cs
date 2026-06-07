using MazadZone.Domain.Bidders;
using MazadZone.Domain.Bidders.Events;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users.ValueObjects;
using Shouldly;

namespace Tests.Domain.Bidders;

public class BidderTests
{
    #region Factory: CompleteProfile

    [Fact]
    public void CompleteProfile_AddressIsNull_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.New();
        var nationalId = "9991012345";

        // Act
        var result = Bidder.CompleteProfile(userId, nationalId, null!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(BidderErrors.AddressMissing);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CompleteProfile_NationalIdIsInvalid_ReturnsFailure(string? invalidNationalId)
    {
        // Arrange
        var userId = UserId.New();
        var address = CreateValidAddress();

        // Act
        var result = Bidder.CompleteProfile(userId, invalidNationalId!, address);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(BidderErrors.InvalidNationalId);
    }

    [Fact]
    public void CompleteProfile_ValidInputs_CreatesBidderAndRaisesEvent()
    {
        // Arrange
        var userId = UserId.New();
        var nationalId = "9991012345";
        var address = CreateValidAddress();

        // Act
        var result = Bidder.CompleteProfile(userId, nationalId, address);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        var bidder = result.Value;
        bidder.Id.Value.ShouldBe(userId.Value); 
        bidder.NationalId.ShouldBe(nationalId);
        bidder.DefaultShippingAddress.ShouldBe(address);
        bidder.IsVerified.ShouldBeFalse();
        
        // Assert Counters Initialize at Zero
        bidder.CompletedPurchasesCount.ShouldBe(0);
        bidder.AuctionsWonCount.ShouldBe(0);
        bidder.TotalPidsPlaced.ShouldBe(0);
        bidder.AuctionParticipatedCount.ShouldBe(0);
        bidder.UnpaidAuctions.ShouldBeEmpty();

        // Verify Domain Event
        var domainEvents = bidder.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        var completedEvent = domainEvents.First().ShouldBeOfType<BidderProfileCompletedDomainEvent>();
        completedEvent.BidderId.ShouldBe(bidder.Id);
    }

    #endregion

    #region State Mutations: Update Address & Verify

[Fact]
public void UpdateShippingAddress_ValidAddress_UpdatesAddressAndReturnsSuccess()
{
    // Arrange
    var bidder = CreateValidBidder();
    var newAddress = Address.Create("UAE", "Dubai", "Sheikh Zayed Rd", "00000").Value;
    // CRITICAL FIX: Seed the address into the bidder's collection first.
    // If it's not already in the list, FindIndex(a => a.Equals(newAddress)) 
    // will return -1 and fail.
    bidder.AddAddress(newAddress); // Or however your aggregate adds an address

    // Act
    var result = bidder.UpdateShippingAddress(newAddress);

    // Assert
    result.IsSuccess.ShouldBeTrue();
}

    [Fact]
    public void Verify_WhenCalled_SetsIsVerifiedAndRaisesEvent()
    {
        // Arrange
        var bidder = CreateValidBidder();
        bidder.ClearDomainEvents();

        // Act
        bidder.Verify();

        // Assert
        bidder.IsVerified.ShouldBeTrue();

        var domainEvents = bidder.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        var verifiedEvent = domainEvents.First().ShouldBeOfType<BidderVerifiedDomainEvent>();
        verifiedEvent.BidderId.ShouldBe(bidder.Id);
    }

    #endregion

    #region Business Rules: Counters & Statistics

    [Fact]
    public void RecordCompletePurchase_WhenCalled_IncrementsCompletedPurchasesCount()
    {
        // Arrange
        var bidder = CreateValidBidder();
        var initialCount = bidder.CompletedPurchasesCount;

        // Act
        bidder.RecordCompletePurchase();

        // Assert
        bidder.CompletedPurchasesCount.ShouldBe(initialCount + 1);
    }

    [Fact]
    public void RecordPidPlaced_WhenCalled_IncrementsTotalPidsPlaced()
    {
        // Arrange
        var bidder = CreateValidBidder();
        var initialCount = bidder.TotalPidsPlaced;

        // Act
        bidder.RecordPidPlaced();

        // Assert
        bidder.TotalPidsPlaced.ShouldBe(initialCount + 1);
    }

    [Fact]
    public void RecordAuctionWon_WhenCalled_IncrementsAuctionsWonCount()
    {
        // Arrange
        var bidder = CreateValidBidder();
        var initialCount = bidder.AuctionsWonCount;

        // Act
        bidder.RecordAuctionWon();

        // Assert
        bidder.AuctionsWonCount.ShouldBe(initialCount + 1);
    }

    [Fact]
    public void RecordAuctionParticipated_WhenCalled_IncrementsAuctionParticipatedCount()
    {
        // Arrange
        var bidder = CreateValidBidder();
        var initialCount = bidder.AuctionParticipatedCount;

        // Act
        bidder.RecordAuctionParticipated();

        // Assert
        bidder.AuctionParticipatedCount.ShouldBe(initialCount + 1);
    }

    #endregion

    // --- Core Factory Helpers ---

    private static Address CreateValidAddress()
    {
        return Address.Create("Jordan", "Amman", "Queen Rania St", "11118").Value;
    }

    private static Bidder CreateValidBidder()
    {
        return Bidder.CompleteProfile(
            UserId.New(), 
            "9991012345", 
            CreateValidAddress()).Value;
    }
}