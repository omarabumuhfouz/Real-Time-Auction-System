using MazadZone.Domain.Payments.ValueObjects;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Orders;
using MazadZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MzadZone.Domain.Payments;

namespace MazadZone.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment, PaymentId>, IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }


    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var typedOrderId = OrderId.From(orderId);

        return await _context.Payments.Include(x => x.Transactions).FirstOrDefaultAsync(x => x.OrderId == typedOrderId);

    }
}