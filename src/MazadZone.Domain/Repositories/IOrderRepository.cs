using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Domain.Repositories;

public interface IOrderRepository : IGenericRepository<Order,OrderId> , IScopedService
{
    Task<Order?> GetWithDispute(OrderId id, CancellationToken ct);
    Task<Order?> GetWithFeedback(OrderId id, CancellationToken ct);
}