using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public interface IPaymentQueries : IScopedService
{
    Task<Money> GetTotalAmountByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}


