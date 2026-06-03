using MazadZone.Domain.Common;
using MazadZone.Domain.Sellers.Events;

namespace MazadZone.Domain.Sellers;

public sealed class Seller : AggregateRoot<UserId>, IAuditableEntity
{
    private Seller() { } 

    private Seller(UserId id) : base(id)
    {
        Rating = 0;
        ReviewsCount = 0;
        IsVerified = false;

        RaiseDomainEvent(new SellerCreatedDomainEvent(Id));
    }
    public decimal Rating { get; private set; }
    public int ReviewsCount { get; private set; }

    public int ListedAuctionsCount { get; private set; }


    public DateTime CreatedOnUtc { get ; set ; }
    public DateTime? ModifiedOnUtc { get ; set ; }

    public bool IsVerified { get; private set; }

    // Domain Logic: Update rating based on a new review
    public void UpdateRating(int newReviewRating)
    {
        // Simple moving average logic
        Rating = ((Rating * ReviewsCount) + newReviewRating) / (ReviewsCount + 1);
        ReviewsCount++;

        RaiseDomainEvent(new SellerRatingUpdatedDomainEvent(Id, Rating));
    }

    public void RecordListedAuction() => ListedAuctionsCount++;



    public void Verify()
    {
        if (IsVerified) return;
        IsVerified = true;
        RaiseDomainEvent(new SellerVerifiedDomainEvent(Id));
    }

    // Logic: Updating banking info (triggers a re-verification check usually)
    public Result UpdateBankDetails()
    {
        IsVerified = false; 

        return Result.Success();
    }

    public static Result<Seller> BecomeSeller(UserId bidderId)
    {
        return new Seller(bidderId);
    }
}