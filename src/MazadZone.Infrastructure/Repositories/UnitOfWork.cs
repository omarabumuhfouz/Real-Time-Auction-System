
using MazadZone.Application.Common.Exceptions;
using MazadZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public UnitOfWork(AppDbContext context) => _context = context;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyConflictException("A concurrency conflict occurred during save.", ex);
        }
    }
}