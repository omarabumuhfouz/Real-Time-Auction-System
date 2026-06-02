
namespace MazadZone.Application.Features.Categories.Queries.GetTrendingRootCategories;

public class GetTrendingRootCategoriesQueryHandler : IQueryHandler<GetTrendingRootCategoriesQuery, IReadOnlyList<CategoryStatResponse>>
{
    private readonly ICategoryQueries _categoryQueries;

    public GetTrendingRootCategoriesQueryHandler(ICategoryQueries categoryQueries)
    {
        _categoryQueries = categoryQueries;
    }

    public async Task<Result<IReadOnlyList<CategoryStatResponse>>> Handle(GetTrendingRootCategoriesQuery request, CancellationToken ct)
    {
        var results = await _categoryQueries.GetRootCategoryStatisticsAsync(
            request.Limit, 
            request.IncludeOther, 
            ct);

        return Result.Success(results);
    }
}