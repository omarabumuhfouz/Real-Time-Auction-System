namespace   MazadZone.Application.Features.Disputes.Queries;

public record DisputeDetailsDto
(
    Guid Id,
    string Status,
    string DisputeType,
    string Title,
    string Description,
    AuctionDisputeInfo AuctionDetails,
    List<DisputeParties> Parties,
   List<DisputeAttachmentDto> Attachments 
);

public record AuctionDisputeInfo
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public decimal FinalPrice { get; init; }
    public DateTime EndTime { get; init; }
    public string MainImageUrl { get; init; } 
}

public record DisputeParties
{
    public DisputeUserDto Bidder { get; init; }
    public DisputeUserDto Seller { get; init; }
}

public record DisputeUserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}

// Block 4: Attachments
public record DisputeAttachmentDto
{
    public string Path { get; init; }
    public string AltText { get; init; }
}