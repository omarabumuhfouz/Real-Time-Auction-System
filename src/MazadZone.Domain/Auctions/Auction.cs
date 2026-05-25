using MazadZone.Domain.Auctions.Enums;
using MazadZone.Domain.Auctions.Events;
using MazadZone.Domain.Categories;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.ValueObjects;

namespace MazadZone.Domain.Auctions;

public sealed class Auction : AggregateRoot<AuctionId>, IAuditableEntity
{

    public IReadOnlyCollection<Bid> Bids => _bids.AsReadOnly();

    // EF Core Constructor
    private Auction() { }

    private Auction(
        AuctionId id,
        Item item,
        UserId sellerId,
        Address shippingAddress,
        Money startBidAmount,
        Money minBidAmount,
        DateTime startTime,
        DateTime endTime) : base(id)
    {

        SellerId = sellerId;
        Item = item;
        ShippingAddress = shippingAddress;
        StartBidAmount = startBidAmount;
        MinBidAmount = minBidAmount;
        StartTime = startTime;
        EndTime = endTime;
        Status = AuctionStatus.Pending; // Default State


        RaiseDomainEvent(new AuctionCreatedDomainEvent(id));
    }

    public Item Item { get; private set; }

    public UserId SellerId { get; private set; }

    public Address ShippingAddress { get; private set; }

    public Money StartBidAmount { get; private set; }
    public Money MinBidAmount { get; private set; } // Acts as the minimum increment

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public AuctionStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    private readonly List<Bid> _bids = new();

    public Reason? CancellationReason { get; private set; } = null;


    // --- Calculated Properties ---
    public Bid? CurrentLeadingBid => _bids.FirstOrDefault(b => b.Status == BidStatus.Leading);
    public Money CurrentHighestBidAmount => CurrentLeadingBid?.Amount ?? StartBidAmount;

    public Money MinNextBidAmount => HasBids
    ? CurrentHighestBidAmount + MinBidAmount
    : StartBidAmount;

    public int TotalBids => _bids.Count;

    public bool HasBids => _bids.Any();



    public TimeSpan GetRemainderTime(DateTime utcNow) =>
        EndTime > utcNow
            ? EndTime - utcNow
            : TimeSpan.Zero;
    // Both the Status must be Active AND the current time must fall within the boundaries
    public bool IsActive(DateTime utcNow) =>
        Status == AuctionStatus.Active &&
        utcNow >= StartTime &&
        utcNow < EndTime;
    public bool IsEnded => Status == AuctionStatus.Ended;

    public bool IsPending(DateTime utcNow) =>
        Status == AuctionStatus.Pending &&
        utcNow < StartTime;

    // --- Factory Method ---
    public static Result<Auction> Create(
        UserId sellerId,
        ItemStatus status,
        Description condition,
        Address shippingAddress,
        decimal startBidAmount,
        decimal minBidAmount,
        DateTime startTime,
        DateTime endTime,
        string title,
        string description,
        List<Image> images,
        CategoryId categoryId
        )
    {
        if (startTime >= endTime) return AuctionErrors.InvalidTimeFrame;

        var minBidResult = Money.Create(minBidAmount, Currency.Jod);
        if (minBidResult.IsFailure) return AuctionErrors.MinBidTooLow;

        var startBidResult = Money.Create(startBidAmount, Currency.Jod);
        if (startBidResult.IsFailure) return AuctionErrors.StartBidTooLow;

        var auctionId = AuctionId.New();


        var CreateItemResult = Item.Create(auctionId, categoryId,status, condition, title, description, images);
        if (CreateItemResult.IsFailure)
            return CreateItemResult.TopError;

        return new Auction(
                auctionId,
                CreateItemResult.Value,
                sellerId,
                shippingAddress,
                startBidResult.Value,
                minBidResult.Value,
                startTime,
                endTime);

    }

    // --- Operations (State Machine) ---

    public Result MarkAsActive(DateTime utcNow)
    {

        if (Status != AuctionStatus.Pending || utcNow >= EndTime)
        {
            return AuctionErrors.CannotStart;
        }

        Status = AuctionStatus.Active;

        RaiseDomainEvent(new AuctionStartedDomainEvent(Id));

        return Result.Success();
    }

    public Result MarkAsEnded(DateTime utcNow)
    {
        if (Status != AuctionStatus.Active)
            return AuctionErrors.CannotEnd;

        Status = AuctionStatus.Ended;

        RaiseDomainEvent(new AuctionEndedDomainEvent(Id));

        return Result.Success();
    }

    public Result MarkAsCancelled(DateTime utcNow, string reason)
    {
        if (Status == AuctionStatus.Cancelled) return Result.Success();

        if (!IsPending(utcNow)) return AuctionErrors.CannotCancel;

        var reasonresult = Reason.Create(reason);
        if (reasonresult.IsFailure) return reasonresult.TopError;

        CancellationReason = reasonresult.Value;
        Status = AuctionStatus.Cancelled;
        RaiseDomainEvent(new AuctionCancelledDomainEvent(Id));

        return Result.Success();
    }

    public Result MarkAsCancelledByAdmin()
    {
        if (Status == AuctionStatus.Cancelled) return AuctionErrors.AlreadyCancelled;

        if (IsEnded) return AuctionErrors.AlreadyEnded;

        Status = AuctionStatus.Cancelled;
        RaiseDomainEvent(new AuctionCancelledDomainEvent(Id));
        return Result.Success();
    }

    // --- Detail Modification ---

    public Result Update(
        decimal startBid,
        decimal minBid,
        DateTime startTime,
        DateTime endTime,
        DateTime utcNow)
    {
        // Business Rule: You can only update details BEFORE the auction starts
        if (!IsPending(utcNow)) return AuctionErrors.CannotUpdateActive;

        if (startTime >= endTime) return AuctionErrors.InvalidTimeFrame;

        var startBidResult = Money.Create(startBid, Currency.Jod);
        if (startBidResult.IsFailure) return startBidResult.TopError;

        var minBidResult = Money.Create(minBid, Currency.Jod);
        if (minBidResult.IsFailure) return minBidResult.TopError;

        StartBidAmount = startBidResult.Value;
        MinBidAmount = minBidResult.Value;
        StartTime = startTime;
        EndTime = endTime;

        return Result.Success();
    }

    // --- Bidding Logic ---
    public Result<Money> ValidateBidEligibility(decimal amount, DateTime utcNow)
    {
        Console.WriteLine($"\n\n\nStatus: {Status}");
        Console.WriteLine($"UtcNow: {utcNow}");
        Console.WriteLine($"StartTime: {StartTime}");
        Console.WriteLine($"EndTime: {EndTime}");
        Console.WriteLine($"IsActive: {IsActive(utcNow)}");
        Console.WriteLine($"IsEnded: {IsEnded}");
        Console.WriteLine($"IsPending: {IsPending(utcNow)}");

        if (Status != AuctionStatus.Active || utcNow < StartTime || utcNow >= EndTime)
            return AuctionErrors.NotActive;
        Console.WriteLine($"\n\n\nStatus: {Status}");
        Console.WriteLine($"UtcNow: {utcNow}");
        Console.WriteLine($"StartTime: {StartTime}");
        Console.WriteLine($"EndTime: {EndTime}");
        Console.WriteLine($"IsActive: {IsActive(utcNow)}");
        Console.WriteLine($"IsEnded: {IsEnded}");
        Console.WriteLine($"IsPending: {IsPending(utcNow)}");

        var amountResult = Money.Create(amount, Currency.Jod);
        if (amountResult.IsFailure)
            return BidErrors.InvalidAmount;

        if (amountResult.Value < MinNextBidAmount)
            return AuctionErrors.BidTooLow;

        var depositAmountResult = Money.Create(amount * AuctionConstants.BidDepositPercentage, Currency.Jod);
        if (depositAmountResult.IsFailure)
            return AuctionErrors.DepositTooLow;

        return Result.Success(depositAmountResult.Value);
    }
    public Result<Bid> PlaceBid(
        UserId bidderId,
        decimal amount,
        string gatewayAuthHoldId,
        DateTime utcNow)
    {
        if (!IsActive(utcNow))
            return AuctionErrors.AlreadyEnded;

        var amountResult = Money.Create(amount, Currency.Jod);
        if (amountResult.IsFailure)
            return BidErrors.InvalidAmount;

        if (amountResult.Value < MinNextBidAmount)
            return AuctionErrors.BidTooLow;

        var depositAmountResult = Money.Create(amount * AuctionConstants.BidDepositPercentage, Currency.Jod);
        if (depositAmountResult.IsFailure)
            return AuctionErrors.DepositTooLow;


        var previousLeadingBid = CurrentLeadingBid;
        if (previousLeadingBid is not null)
        {
            previousLeadingBid.MarkAsOutbid();
            RaiseDomainEvent(new BidderOutbidDomainEvent(Id, previousLeadingBid.Id, previousLeadingBid.BidderId, previousLeadingBid.Amount));
        }

        // create the new bid in the domain aggregate and pass the authhold
        var newBid = Bid.Create(
            Id,
            bidderId,
            amountResult.Value,
            depositAmountResult.Value,
            gatewayAuthHoldId);

        newBid.MarkAsLeading();
        _bids.Add(newBid);

        RaiseDomainEvent(new BidPlacedDomainEvent(Id, newBid.Id));

        return Result.Success(newBid);
    }
    public void RemoveBidsByBidder(UserId bidderId)
    {
        var bidderBids = _bids.Where(b => b.BidderId == bidderId.Value).ToList();

        if (!bidderBids.Any()) return;

        bool removedLeadingBid = bidderBids.Any(b => b.Status == BidStatus.Leading);

        // Remove bids for bidder
        foreach (var bid in bidderBids)
        {
            _bids.Remove(bid);
        }

        //if removed leading bid
        if (removedLeadingBid)
        {
            // search on last bids 
            var nextLeadingBid = _bids
                .Where(b => b.Status == BidStatus.Outbid)
                .OrderByDescending(b => b.Amount.Amount)
                .FirstOrDefault();

            if (nextLeadingBid != null)
            {
                nextLeadingBid.MarkAsLeading();
            }
        }
    }


}