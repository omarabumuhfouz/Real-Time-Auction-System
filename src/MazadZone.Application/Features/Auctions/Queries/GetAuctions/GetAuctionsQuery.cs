using MazadZone.Application.Common.Paging;
using MazadZone.Domain.Categories;

namespace MazadZone.Application.Features.Auctions.Queries;

public sealed record GetAuctionsQuery(
    int Page = 1, // default to first page
    int PageSize = 12, // default page size, can be adjusted as needed

    string? SearchTerm = null, // search by item title or description
    CategoryId? CategoryId = null, // filter by category
    CategoryId? SubCategoryId = null, // filter by subcategory

    CurrentBidAmountRange? CurrentBidAmount = null, // filter auctions with Current bid amount greater than or equal to this value
    string? Status = "active", // filter by auction status: "active", "Pending", "Ended", "all", 

    string SortBy = "CreationDate", // "CreationDate","StartTime", "EndTime", "StartAmount" , "CurrentBidAmount"
    string SortDirection = "desc", // "asc" or "desc" 

    string? ItemStatus = null, // filter by item status: "New", "Good" .... 
    string? Condition = null // filter by condition 
) : ICachedQuery<PagedList<AuctionsListDto>>
{
    public string CacheKey =>
        $"auctions:p={Page}:ps={PageSize}" +
        $":s={SearchTerm ?? "all"}" +
        $":cat={CategoryId?.ToString() ?? "all"}" +
        $":subcat={SubCategoryId?.ToString() ?? "all"}" +
        $":CurrentBidAmount={CurrentBidAmount?.ToString() ?? "any"}" +
        $":status={Status}" +
        $":sort={SortBy}:{SortDirection}";

    public string[] Tags => ["auctions"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(2);
}