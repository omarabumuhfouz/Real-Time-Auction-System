namespace MazadZone.Application.Features.Categories.Queries.GetCategoryStatistics;

public record GetCategoryStatisticsQuery(int Limit, bool IncludeOther) : IQuery<IReadOnlyList<CategoryStatResponse>>;