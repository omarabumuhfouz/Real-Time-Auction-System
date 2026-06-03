using MazadZone.Domain.Users;

namespace MazadZone.Domain.Bidders;

/// <summary>
/// Represents the identity verification record for a Bidder.
/// Owned by the Bidder aggregate root; persisted in a separate table.
/// </summary>
public sealed class BidderVerification
{
    private BidderVerification() { }

    internal BidderVerification(string nationalId)
    {
        NationalId = nationalId;
        Status = VerificationStatus.Unverified;
        IsVerified = false;
    }

    public string NationalId { get; private set; } = null!;
    public bool IsVerified { get; private set; }
    public VerificationStatus Status { get; private set; } = VerificationStatus.Unverified;
    public string? ExtractedFullName { get; private set; }
    public string? RejectionReason { get; private set; }

    internal void SubmitForVerification()
    {
        Status = VerificationStatus.Pending;
        RejectionReason = null;
    }

    internal void Approve(string nationalId, string fullName)
    {
        NationalId = nationalId;
        ExtractedFullName = fullName;
        Status = VerificationStatus.Verified;
        RejectionReason = null;
        IsVerified = true;
    }

    internal void Reject(string reason)
    {
        Status = VerificationStatus.Rejected;
        RejectionReason = reason;
        ExtractedFullName = null;
        IsVerified = false;
    }
}
