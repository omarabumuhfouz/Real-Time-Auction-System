using MazadZone.Domain.Orders;
using MazadZone.Domain.Repositories;
using MazadZone.Infrastructure.Persistence;
using MazadZone.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Repositories;

public class OrderRepository :  GenericRepository<Order, OrderId>,IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetWithDispute(OrderId id, CancellationToken ct)
    {
        return await _dbContext.Orders
            .Include(o => o.Dispute)
            .FindByIdAsync(id, ct);
    }

    public Task<Order?> GetWithFeedback(OrderId id, CancellationToken ct)
    {
        return _dbContext.Orders
            .Include(o => o.Feedback)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }
}