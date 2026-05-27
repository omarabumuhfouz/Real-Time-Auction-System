namespace MazadZone.Application.Features.Categories.Queries.GetTrendingCategoriesWithAuctionCount;

public record GetTrendingCategoriesWithAuctionCountQuery(int Limit = 10) 
    : IQuery<IReadOnlyList<TrendingCategoryAuctionCountResponse>>;