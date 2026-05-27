using MazadZone.Domain.Categories;

namespace MazadZone.Application.Features.Auctions.Queries;

public record AuctionQueryParameters
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public string? SearchTerm { get; init; }
    public CategoryId? CategoryId { get; init; }

    public CurrentBidAmountRange? CurrentBidAmount { get; init; }
    public string Status { get; init; }
    public string SortBy { get; init; }
    public string? SortDirection { get; init; }
    public string? ItemStatus { get; init; }
    public string? Condition { get; init; }
}

public class CurrentBidAmountRange
{
    public decimal? Min { get; init; }
    public decimal? Max { get; init; }
}

//_dbcontext.auctions.firstordfualt(a=>a.id==auctionid).select