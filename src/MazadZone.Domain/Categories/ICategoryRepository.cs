using MazadZone.Domain.Repositories;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Domain.Categories;
public interface ICategoryRepository : IGenericRepository<Category, CategoryId>, IScopedService
{
    Task<bool> ExistsAsync(CategoryId id, CancellationToken ct);
    Task<Category?> GetByIdForRestoreAsync(CategoryId id, CancellationToken ct);

    // Task<bool> IsNameUniqueAsync(string name, CategoryId? parentId, CancellationToken ct);


    Task<bool> ExistsWithNameInScopeAsync(
        string name,
        CategoryId? parentId,
        CategoryId? excludeId,
        CancellationToken ct);

}