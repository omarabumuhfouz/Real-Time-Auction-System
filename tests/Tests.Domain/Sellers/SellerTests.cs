using MazadZone.Domain.Sellers;
using MazadZone.Domain.Sellers.Events;
using MazadZone.Domain.Users.ValueObjects; // Assuming UserId resides here based on old tests
using Shouldly;

namespace Tests.Domain.Sellers;

public class SellerTests
{
    #region Factory Method: BecomeSeller

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BecomeSeller_NationalIdIsInvalid_ReturnsInvalidNationalIdError(string? invalidNationalId)
    {
        // Arrange
        var bidderId = UserId.New();

        // Act
        var result = Seller.BecomeSeller(bidderId, invalidNationalId!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(SellerErrors.InvalidNationalId);
    }

    [Fact]
    public void BecomeSeller_ValidInputs_CreatesSellerAndRaisesSellerCreatedDomainEvent()
    {
        // Arrange
        var bidderId = UserId.New();
        var nationalId = "9991012345";

        // Act
        var result = Seller.BecomeSeller(bidderId, nationalId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        var seller = result.Value;
        seller.Id.Value.ShouldBe(bidderId.Value);
        seller.NationalId.ShouldBe(nationalId);
        seller.IsVerified.ShouldBeFalse();
        seller.Rating.ShouldBe(0);
        seller.ReviewsCount.ShouldBe(0);
        seller.ListedAuctionsCount.ShouldBe(0);

        // Verify Domain Event
        var domainEvents = seller.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        var createdEvent = domainEvents.First().ShouldBeOfType<SellerCreatedDomainEvent>();
        createdEvent.SellerId.ShouldBe(seller.Id);
    }

    #endregion

    #region Method: UpdateBankDetails

    [Fact]
    public void UpdateBankDetails_WhenCalled_RevokesVerificationStatus()
    {
        // Arrange
        var seller = CreateValidSeller();
        seller.Verify(); // Force verification status to true
        seller.ClearDomainEvents(); 

        // Act
        var result = seller.UpdateBankDetails();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        // Critical Business Rule: Updating bank info revokes verified status
        seller.IsVerified.ShouldBeFalse(); 
    }

    #endregion

    #region Method: Verify

    [Fact]
    public void Verify_UnverifiedSeller_SetsVerifiedFlagAndRaisesSellerVerifiedDomainEvent()
    {
        // Arrange
        var seller = CreateValidSeller();
        seller.ClearDomainEvents(); // Clear construction creation event

        // Act
        seller.Verify();

        // Assert
        seller.IsVerified.ShouldBeTrue();

        // Verify Domain Event
        var domainEvents = seller.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        var verifiedEvent = domainEvents.First().ShouldBeOfType<SellerVerifiedDomainEvent>();
        verifiedEvent.SellerId.ShouldBe(seller.Id);
    }

    [Fact]
    public void Verify_AlreadyVerifiedSeller_DoesNotRaiseDuplicateDomainEvent()
    {
        // Arrange
        var seller = CreateValidSeller();
        seller.Verify(); // First verification
        seller.ClearDomainEvents(); // Wipe out the events baseline

        // Act
        seller.Verify(); // Second verification call

        // Assert
        seller.IsVerified.ShouldBeTrue();
        seller.DomainEvents.Count.ShouldBe(0); // Guard clause should have hit `return;`
    }

    #endregion

    #region Method: UpdateRating

    [Fact]
    public void UpdateRating_ValidStarRatings_CalculatesMovingAverageAndRaisesSellerRatingUpdatedDomainEvents()
    {
        // Arrange
        var seller = CreateValidSeller();
        seller.ClearDomainEvents();

        // Act & Assert 1: First Review (5 stars)
        // Math: ((0 * 0) + 5) / (0 + 1) = 5
        seller.UpdateRating(5);
        seller.Rating.ShouldBe(5m);
        seller.ReviewsCount.ShouldBe(1);

        // Act & Assert 2: Second Review (3 stars)
        // Math: ((5 * 1) + 3) / (1 + 1) = 4
        seller.UpdateRating(3);
        seller.Rating.ShouldBe(4m);
        seller.ReviewsCount.ShouldBe(2);

        // Act & Assert 3: Third Review (1 star)
        // Math: ((4 * 2) + 1) / (2 + 1) = 3
        seller.UpdateRating(1);
        seller.Rating.ShouldBe(3m);
        seller.ReviewsCount.ShouldBe(3);

        // Verify Domain Events
        var domainEvents = seller.DomainEvents;
        domainEvents.Count.ShouldBe(3); // One event for each update
        
        var latestEvent = domainEvents.Last().ShouldBeOfType<SellerRatingUpdatedDomainEvent>();
        latestEvent.SellerId.ShouldBe(seller.Id);
        latestEvent.NewRating.ShouldBe(3m);
    }

    #endregion

    #region Method: RecordListedAuction

    [Fact]
    public void RecordListedAuction_WhenCalled_IncrementsListedAuctionsCount()
    {
        // Arrange
        var seller = CreateValidSeller();
        seller.ListedAuctionsCount.ShouldBe(0);

        // Act
        seller.RecordListedAuction();
        seller.RecordListedAuction();

        // Assert
        seller.ListedAuctionsCount.ShouldBe(2);
    }

    #endregion

    // --- Helpers ---
    private static Seller CreateValidSeller()
    {
        return Seller.BecomeSeller(
            UserId.New(), 
            "9991012345").Value;
    }
}