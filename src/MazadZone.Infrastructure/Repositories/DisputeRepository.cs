using MazadZone.Domain.Disputes;
using MazadZone.Domain.Orders;
using MazadZone.Infrastructure.Persistence;

namespace MazadZone.Infrastructure.Repositories;

public class DisputeRepository :GenericRepository<Dispute, DisputeId>, IDisputeRepository
{
    private readonly AppDbContext _context;

    public DisputeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Dispute?> GetByOrderIdAsync(OrderId orderId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}