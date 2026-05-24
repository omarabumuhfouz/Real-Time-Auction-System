using MazadZone.Application.Services;
using MazadZone.Domain.Orders;
using MazadZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MazadZone.Infrastructure.Services;

public sealed class PaymentQueries : IPaymentQueries
{
    private readonly AppDbContext _dbContext;

    public PaymentQueries(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Money> GetTotalAmountByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(o => o.Id.Value == orderId, cancellationToken);

        return order?.TotalAmount ?? throw new InvalidOperationException($"Order not found for OrderId {orderId}");
    }
}
