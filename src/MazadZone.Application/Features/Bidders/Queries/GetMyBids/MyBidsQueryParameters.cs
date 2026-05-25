namespace MazadZone.Application.Features.Bidders.Queries.GetMyBids;

public sealed record MyBidsQueryParameters
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? SearchTerm { get; init; }
    public Guid? CategoryId { get; init; }
    public string Tab { get; init; } = "all";
    public string SortBy { get; init; } = "EndTime";
    public string SortDirection { get; init; } = "desc";
}
