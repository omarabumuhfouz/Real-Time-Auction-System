using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Bidders.Events;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Domain.ValueObjects;
using Shouldly;

namespace Tests.Domain.Bidders;

public class BidderTests
{
    #region Factory: CompleteProfile

    [Fact]
    public void CompleteProfile_Should_ReturnFailure_When_AddressIsNull()
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
    public void CompleteProfile_Should_ReturnFailure_When_NationalIdIsInvalid(string? invalidNationalId)
    {
        // Arrange
        var userId = UserId.New();
        var address = CreateValidAddress();

        // Act
        var result = Bidder.CompleteProfile(userId, invalidNationalId, address);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(BidderErrors.InvalidNationalId);
    }

    [Fact]
    public void CompleteProfile_Should_CreateBidder_And_RaiseEvent_When_Valid()
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
        bidder.Id.Value.ShouldBe(userId.Value); // Ensure ID mapped perfectly
        bidder.NationalId.ShouldBe(nationalId);
        bidder.DefaultShippingAddress.ShouldBe(address);
        bidder.IsVerified.ShouldBeFalse();
        bidder.TotalAmountSpent.Amount.ShouldBe(0);
        bidder.ActiveBidsTotal.Amount.ShouldBe(0);

        // Verify Domain Event
        var domainEvents = bidder.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        var completedEvent = domainEvents.First().ShouldBeOfType<BidderProfileCompletedDomainEvent>();
        completedEvent.BidderId.ShouldBe(bidder.Id);
    }

    #endregion

    #region State Mutations: Update Address & Verify

    [Fact]
    public void UpdateShippingAddress_Should_ReplaceAddress_And_ReturnSuccess()
    {
        // Arrange
        var bidder = CreateValidBidder();
        var newAddress = Address.Create("UAE", "Dubai", "Sheikh Zayed Rd", "00000").Value;

        // Act
        var result = bidder.UpdateShippingAddress(newAddress);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        bidder.DefaultShippingAddress.ShouldBe(newAddress);
    }

    [Fact]
    public void Verify_Should_SetVerifiedFlag_And_RaiseEvent()
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
        domainEvents.First().ShouldBeOfType<BidderVerifiedDomainEvent>();
    }

    [Fact]
    public void RecordPaymentSuccess_Should_IncrementSuccessfulPaymentsCounter()
    {
        // Arrange
        var bidder = CreateValidBidder();

        // Act
        bidder.RecordPaymentSuccess(Money.Create(100, Currency.Jod).Value);

        // Assert
        bidder.SuccessfulPayments.ShouldBe(1);
    }

    #endregion

    #region Business Rules: Bidding Limits

    [Fact]
    public void AddActiveBid_Should_IncreaseTotal_When_WithinCreditLimit()
    {
        // Arrange
        var bidder = CreateValidBidder();
        
        // Assuming DefaultCreditLimit is a decimal, adjust object creation if Money works differently
        var safeAmount = Money.Create(BidderPolicies.DefaultCreditLimit - 100, Currency.Jod); 

        // Act
        var result = bidder.AddActiveBid(safeAmount.Value);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        bidder.ActiveBidsTotal.Amount.ShouldBe(safeAmount.Value.Amount);
    }

    [Fact]
    public void AddActiveBid_Should_ReturnFailure_And_NotMutateState_When_ExceedingLimit()
    {
        // Arrange
        var bidder = CreateValidBidder();
        var existingTotal = bidder.ActiveBidsTotal.Amount;
        
        var excessiveAmount = Money.Create(BidderPolicies.DefaultCreditLimit + 100, Currency.Jod);

        // Act
        var result = bidder.AddActiveBid(excessiveAmount.Value);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(BidderErrors.CreditLimitReached);
        
        // Critical: Ensure the state was NOT modified if the rule failed
        bidder.ActiveBidsTotal.Amount.ShouldBe(existingTotal);
    }

    #endregion

    #region Business Rules: Unpaid Auctions

    [Fact]
    public void RecordNonPayment_Should_AddAuction_And_RaiseFailedToPayEvent()
    {
        // Arrange
        var bidder = CreateValidBidder();
        bidder.ClearDomainEvents();
        var auctionId = AuctionId.New();

        // Act
        bidder.RecordNonPayment(auctionId);

        // Assert
        bidder.UnpaidWins.ShouldBe(1);
        bidder.UnpaidAuctions.ShouldContain(auctionId);

        var domainEvents = bidder.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        
        var failedEvent = domainEvents.First().ShouldBeOfType<BidderFailedToPayDomainEvent>();
        failedEvent.AuctionId.ShouldBe(auctionId);
        failedEvent.CurrentUnpaidCount.ShouldBe(1);
    }

    [Fact]
    public void RecordNonPayment_Should_IgnoreDuplicateAuctions()
    {
        // Arrange
        var bidder = CreateValidBidder();
        bidder.ClearDomainEvents();
        var auctionId = AuctionId.New();

        // Act
        bidder.RecordNonPayment(auctionId);
        bidder.ClearDomainEvents(); // Clear the first event
        
        // Try to record the exact same auction again
        bidder.RecordNonPayment(auctionId);

        // Assert
        // UnpaidWins should still be 1, because the HashSet rejected the duplicate
        bidder.UnpaidWins.ShouldBe(1);
        
        // Ensure no extra events were fired
        bidder.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void RecordNonPayment_Should_RaiseExceededLimitEvent_When_ThresholdReached()
    {
        // Arrange
        var bidder = CreateValidBidder();
        bidder.ClearDomainEvents();

        // Act
        // Loop up to the exact threshold
        for (int i = 0; i < BidderPolicies.MaxUnpaidWinsThreshold; i++)
        {
            bidder.RecordNonPayment(AuctionId.New());
        }

        // Assert
        bidder.UnpaidWins.ShouldBe(BidderPolicies.MaxUnpaidWinsThreshold);

        var domainEvents = bidder.DomainEvents;
        
        // Ensure the critical suspension event was fired at the end
        domainEvents.Last().ShouldBeOfType<BidderExceededUnpaidLimitDomainEvent>();
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