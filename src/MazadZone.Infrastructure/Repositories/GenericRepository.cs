using System.Linq.Expressions;
using MazadZone.Domain.Primitives;
using MazadZone.Domain.Repositories;
using MazadZone.Infrastructure.Persistence;
using MazadZone.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Repositories;

public class GenericRepository<T,TId> : IGenericRepository<T, TId> 
where T : Entity<TId>
where TId : notnull
{
    protected readonly AppDbContext DbContext;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(AppDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
    } 

    public void Add(T entity)
    {
        // Using synchronous Add as discussed!
        DbSet.Add(entity);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }

    public virtual async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindByIdAsync(id, cancellationToken);
    }
}