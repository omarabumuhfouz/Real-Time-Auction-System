using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders.Events;
using MazadZone.Domain.Common;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users;

namespace MazadZone.Domain.Bidders;

public sealed class Bidder : AggregateRoot<UserId>, IAuditableEntity, IVerifiableEntity
{
    public IReadOnlyCollection<AuctionId> UnpaidAuctions => _unpaidAuctions;

    private Bidder() { } 
    private Bidder(UserId id, string nationalId, Address defaultShippingAddress) : base(id)
    {
        DefaultShippingAddress = defaultShippingAddress;
        Verification = new BidderVerification(nationalId);
    }

    public Address DefaultShippingAddress { get; private set; }

    public int CompletedPurchasesCount { get; private set; }
    public int AuctionsWonCount { get; private set; }
    public int TotalPidsPlaced { get; private set; }
    public int AuctionParticipatedCount { get; private set; }

    /// <summary>
    /// Owned entity containing all identity verification state.
    /// Persisted in the BidderVerifications table.
    /// </summary>
    public BidderVerification Verification { get; private set; } = null!;

    // IVerifiableEntity implementation – delegates to the owned entity
    public bool IsVerified => Verification.IsVerified;
    public string NationalId => Verification.NationalId;

    public DateTime CreatedOnUtc { get ; set ; }
    public DateTime? ModifiedOnUtc { get ; set ; }

    private readonly HashSet<AuctionId> _unpaidAuctions = new();

    public void RecordCompletePurchase() => CompletedPurchasesCount++; 
    public void RecordPidPlaced() => TotalPidsPlaced++;
    public void RecordAuctionWon() => AuctionsWonCount++; 
    public void RecordAuctionParticipated() => AuctionParticipatedCount++;

    
    public static Result<Bidder> CompleteProfile(UserId userId, string nationalId, Address defaultShippingAddress)
    {
        if (defaultShippingAddress is null) return BidderErrors.AddressMissing;

        if(string.IsNullOrWhiteSpace(nationalId)) return BidderErrors.InvalidNationalId;

        var bidder = new Bidder(userId, nationalId, defaultShippingAddress);

        bidder.RaiseDomainEvent(new BidderProfileCompletedDomainEvent(userId));

        return bidder;
    }

    public Result UpdateShippingAddress(Address newAddress)
    {
        DefaultShippingAddress = newAddress;
        return Result.Success();
    }

    public void Verify()
    {
        Verification.Approve(Verification.NationalId, Verification.ExtractedFullName ?? string.Empty);
        RaiseDomainEvent(new BidderVerifiedDomainEvent(this.Id));
    }

    public void SubmitForVerification()
    {
        Verification.SubmitForVerification();
    }

    public void ApproveVerification(string nationalId, string fullName)
    {
        Verification.Approve(nationalId, fullName);
        RaiseDomainEvent(new BidderVerifiedDomainEvent(this.Id));
    }

    public void RejectVerification(string reason)
    {
        Verification.Reject(reason);
    }
}