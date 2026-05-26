using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Domain.Repositories;

public interface IGenericRepository<T, TId> : IScopedService  
where T :  Entity<TId>
where TId : notnull
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    
    void Add(T entity);
    
    void Update(T entity);
    
    void Delete(T entity);
}