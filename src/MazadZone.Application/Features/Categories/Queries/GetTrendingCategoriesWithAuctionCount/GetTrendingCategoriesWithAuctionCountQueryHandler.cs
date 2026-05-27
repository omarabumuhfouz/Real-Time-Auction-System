namespace MazadZone.Application.Features.Categories.Queries.GetTrendingCategoriesWithAuctionCount;

public sealed class GetTrendingCategoriesWithAuctionCountQueryHandler 
    : IQueryHandler<GetTrendingCategoriesWithAuctionCountQuery, IReadOnlyList<TrendingCategoryAuctionCountResponse>>
{
    private readonly ICategoryQueries _categoryQueries;
    private readonly ILogger<GetTrendingCategoriesWithAuctionCountQueryHandler> _logger;

    public GetTrendingCategoriesWithAuctionCountQueryHandler(
        ICategoryQueries categoryQueries, 
        ILogger<GetTrendingCategoriesWithAuctionCountQueryHandler> logger)
    {
        _categoryQueries = categoryQueries;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<TrendingCategoryAuctionCountResponse>>> Handle(
        GetTrendingCategoriesWithAuctionCountQuery request, 
        CancellationToken ct)
    {
        _logger.LogInformation("Processing trend analysis metrics with selection cap window of {Limit}", request.Limit);
        
        var trendingStats = await _categoryQueries.GetTrendingCategoriesWithAuctionCountAsync(request.Limit, ct);
        
        _logger.LogInformation("Successfully mapped out {Count} active trending categories with live action metrics.", trendingStats.Count);
        
        return Result.Success(trendingStats);
    }
}