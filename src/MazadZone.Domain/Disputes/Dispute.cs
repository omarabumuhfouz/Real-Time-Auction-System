namespace MazadZone.Domain.Disputes;

using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Primitives;
using MazadZone.Domain.Shared.ValueObjects;

public sealed class Dispute : AggregateRoot<DisputeId>
{
    private Dispute() { }

    private Dispute(DisputeId id, OrderId orderId,DisputeTypeId disputeTypeId, Description description, Title title, List<Image>? images = null) : base(id)
    {
        OrderId = orderId;
        Status = DisputeStatus.Open; 
        CreatedAtUtc = DateTime.UtcNow;
        Status = DisputeStatus.Open;
        Description = description;
        Title = title;
        DisputeTypeId = disputeTypeId;
        Images = images ?? new List<Image>();
    }

    public OrderId OrderId { get; private init; }
    public DisputeTypeId DisputeTypeId { get; private init; }
    public Title Title { get; private set; }
    public Description Description { get; private set; }
    public Resolution Resolution { get; private set; } = Resolution.Empty;
    public DisputeStatus Status { get; private set; }
    public List<Image> Images { get; private set; } = new();
    
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime? ResolvedAtUtc { get; private set; }


    public bool IsResolved => Status == DisputeStatus.Resolved;

    public static Dispute Open(OrderId orderId,DisputeTypeId disputeTypeId,Title title,Description description, List<Image>? images = null)
    {
        var dispute = new Dispute(DisputeId.New(), orderId, disputeTypeId, description, title, images);

        dispute.RaiseDomainEvent(new DisputeOpenedDomainEvent(dispute.Id, orderId));
        return dispute;
    }

    /// <summary>
    /// Resolves an existing dispute by delegating the logic to the <see cref="Dispute"/> entity.
    /// </summary>
    public Result Resolve(Resolution resolution)
    {
        if (Status == DisputeStatus.Resolved) return Result.Success();

        Resolution = resolution;
        RaiseDomainEvent(new DisputeResolvedDomainEvent(this.Id, OrderId, resolution.Value));
        return Result.Success();
    }
    
    public Result UnderReview()
    {
        if (Status == DisputeStatus.UnderReview) return Result.Success();

        if (Status == DisputeStatus.Resolved) return Result.Success();

        Status = DisputeStatus.UnderReview;
        return Result.Success();
    }


    internal Result ChangeDescription(Description description)
    {
        if (Status == DisputeStatus.Resolved)
            return OrderErrors.DisputeCannotChangeReason;

        Description = description;
        return Result.Success();
    }

    internal Result Resolve(string resolutionText)
    {
        if (Status == DisputeStatus.Resolved)
            return OrderErrors.DisputeAlreadyResolved;

        var resolutionResult = Resolution.Create(resolutionText);
        if (resolutionResult.IsFailure) return resolutionResult.TopError;

        Resolution = resolutionResult.Value;
        Status = DisputeStatus.Resolved;
        ResolvedAtUtc = DateTime.UtcNow; 
        
        return Result.Success();
    }
}
