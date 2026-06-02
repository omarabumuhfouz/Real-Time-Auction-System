using MazadZone.Domain.Categories;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Features.Categories.Queries;

public interface ICategoryQueries : IScopedService
{
    Task<IReadOnlyList<CategoryResponse>> GetRootCategoriesAsync(CancellationToken ct);
    Task<IReadOnlyList<CategoryResponse>> GetSubCategoriesAsync(CategoryId parentId, CancellationToken ct);
    Task<CategoryResponse?> GetByIdAsync(CategoryId id, CancellationToken ct);
    Task<IReadOnlyList<CategoryTreeResponse>> GetTreeAsync(CancellationToken ct);
    Task<IReadOnlyList<BreadcrumbResponse>> GetBreadcrumbsAsync(CategoryId id, CancellationToken ct);

    Task<IReadOnlyList<CategoryStatResponse>> GetCategoryStatisticsAsync(int limit, bool includeOther, CancellationToken ct);
    Task<IReadOnlyList<CategoryResponse>> SearchByNameAsync(string name, CancellationToken ct);
    Task<IReadOnlyList<CategoryStatResponse>> GetRootCategoryStatisticsAsync(
    int limit,
    bool includeOther,
    CancellationToken ct);
}

public record CategoryResponse(Guid Id, string Name, string Description, Guid? ParentId);
public record CategoryTreeResponse(Guid Id, string Name, string Description, Guid? ParentId)
{
    public List<CategoryTreeResponse> Children { get; init; } = new();
}

public record BreadcrumbResponse(Guid Id, string Name, int Level);
public record CategoryStatResponse(Guid? Id, string Name, int ActiveAuctionsCount)
{
    public bool IsOtherBucket => !Id.HasValue;
}
public record TrendingCategoryResponse(Guid Id, string Name, int InteractionCount);
public record CategoryDetailsResponse(Guid Id, string Name, string Description, Guid? ParentId, string? ParentName, int ChildCount);