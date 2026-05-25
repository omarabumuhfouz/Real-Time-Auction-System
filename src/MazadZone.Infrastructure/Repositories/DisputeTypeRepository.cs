using MazadZone.Domain.Disputes;
using MazadZone.Domain.Orders;
using MazadZone.Infrastructure.Persistence;

namespace MazadZone.Infrastructure.Repositories;

public class DisputeTypeRepository : GenericRepository<DisputeType, DisputeTypeId>, IDisputeTypeRepository
{
    private readonly AppDbContext _context;
    public DisputeTypeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
}