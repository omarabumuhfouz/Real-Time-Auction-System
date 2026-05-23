using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MazadZone.Domain.Primitives;

namespace MazadZone.Infrastructure.Persistence.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Finds an entity by its strongly-typed ID without boxing and bypassing EF Core's .Equals translation bug.
    /// </summary>
    public static Task<T?> FindByIdAsync<T, TId>(
        this IQueryable<T> query, 
        TId id, 
        CancellationToken cancellationToken = default) 
        where T : Entity<TId>
        where TId : notnull
    {
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, nameof(Entity<TId>.Id));
        var constant = Expression.Constant(id, typeof(TId));
        
        var body = Expression.Equal(property, constant);
        var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);

        return query.FirstOrDefaultAsync(predicate, cancellationToken);
    }
}