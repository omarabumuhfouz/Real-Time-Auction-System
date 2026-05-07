using MazadZone.Domain.Entities.Orders;
using MazadZone.Domain.Orders;
using MazadZone.Infrastructure.Persistence;

namespace MazadZone.Infrastructure.Repositories;

public class OrderRepository :  GenericRepository<Order>,IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

}