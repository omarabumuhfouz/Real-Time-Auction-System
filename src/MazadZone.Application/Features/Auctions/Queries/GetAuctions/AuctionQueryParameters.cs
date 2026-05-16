namespace MazadZone.Application.Features.Auctions.Queries;

public record AuctionQueryParameters
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? SubCategoryId { get; init; }
    public decimal? CurrentBidAmount { get; init; }
    public string Status { get; init; } = "active";
    public string SortBy { get; init; } = "CreationDate";
    public string? SortDirection { get; init; } = "desc";
}

//_dbcontext.auctions.firstordfualt(a=>a.id==auctionid).select