using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders.Events;
using MazadZone.Domain.Common;
using MazadZone.Domain.Shared.ValueObjects;

namespace MazadZone.Domain.Bidders;

public sealed class Bidder : AggregateRoot<UserId>, IAuditableEntity, IVerifiableEntity
{
    public IReadOnlyCollection<AuctionId> UnpaidAuctions => _unpaidAuctions;

    private Bidder() { } 
    private Bidder(UserId id, string nationalId, Address defaultShippingAddress) : base(id)
    {
        DefaultShippingAddress = defaultShippingAddress;
        NationalId = nationalId;
        IsVerified = false;

    }

    public Address DefaultShippingAddress { get; private set; }

    public int CompletedPurchasesCount { get; private set; }
    public int AuctionsWonCount { get; private set; }
    public int TotalPidsPlaced { get; private set; }
    public int AuctionParticipatedCount { get; private set; }

    public bool IsVerified { get; private set; }
    public string NationalId { get; private set; }

    public DateTime CreatedOnUtc { get ; set ; }
    public DateTime? ModifiedOnUtc { get ; set ; }

    private readonly HashSet<AuctionId> _unpaidAuctions = new();

    public void RecordCompletePurchase() => CompletedPurchasesCount++;
    public void RecordPidPlaced() => TotalPidsPlaced++;
    public void RecordAuctionWon() => AuctionsWonCount++;
    public void RecordAuctionParticipated() => AuctionParticipatedCount++;

    
    public static Result<Bidder> CompleteProfile(UserId userId,string nationalId, Address defaultShippingAddress)
    {
        if (defaultShippingAddress is null) return BidderErrors.AddressMissing;

        if(string.IsNullOrWhiteSpace(nationalId)) return BidderErrors.InvalidNationalId;

        var bidder = new Bidder(userId,nationalId, defaultShippingAddress);

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
        IsVerified = true;
        RaiseDomainEvent(new BidderVerifiedDomainEvent(this.Id));
    }
}