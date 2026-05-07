using MazadZone.Application.Common.Paging;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Extensions;

public static class PagedListExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source, 
        int pageNumber, 
        int pageSize, 
        CancellationToken ct = default)
    {
        // 1. Execute the count query on the database
        var count = await source.CountAsync(ct);

        // 2. Execute the pagination query on the database
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct); // Materializes to a List<T>

        // 3. Return the fully formed DTO wrapper
        return new PagedList<T>(items, pageNumber, pageSize, count);
    }
}