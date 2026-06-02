namespace MazadZone.Application.Features.Categories.Queries.GetTrendingRootCategories;

public record GetTrendingRootCategoriesQuery(
    int Limit, 
    bool IncludeOther
) : IQuery<IReadOnlyList<CategoryStatResponse>>;