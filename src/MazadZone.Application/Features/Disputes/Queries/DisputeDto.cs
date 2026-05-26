namespace MazadZone.Application.Features.Disputes.Queries;


public record DisputeListItemDto
{
    // The internal ID is required for the "View" action button to work
    public Guid Id { get; init; } 

    // Parties
    public string BidderName { get; init; }
    public string SellerName { get; init; }

    // Dispute Info
    public string Category { get; init; }
    public string Status { get; init; }
    public DateTime SubmittedDate { get; init; }
}