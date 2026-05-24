using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Orders;
using Microsoft.Extensions.Logging;
using MzadZone.Domain.Payments;

namespace MazadZone.Infrastructure.Services;

public class PaymentService(ILogger<PaymentService> _logger) : IPaymentService
{

    public async Task<string?> CaptureHoldedAmountAsync(string gatewayIntentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Capturing hold {GatewayIntentId}", gatewayIntentId);
        
        await Task.Delay(500, cancellationToken);
        
        return $"cap_mock_{Guid.NewGuid():N}";
    }

    public async Task UnAuthorizeAsync(string gatewayIntentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Unauthorizing hold {GatewayIntentId}", gatewayIntentId);
        
        await Task.Delay(500, cancellationToken);
    }

    public async Task<string> AuthorizeAsync(Guid userId, Guid auctionId, string methodId, Money depositAmount, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Authorizing {Amount} for User {UserId}.", depositAmount, userId);
        
        await Task.Delay(800, cancellationToken); 

        return $"auth_mock_{Guid.NewGuid():N}";
    }

    public async Task<string?> ChargeAmountAsync(Guid userId, Money Amount, string paymentMethodId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MOCK: Charging {Amount} to User {UserId}.", Amount.Amount, userId);
        
        await Task.Delay(1000, cancellationToken);

        if (Amount.Amount == 9999m)
        {
            _logger.LogWarning("MOCK: Simulated card decline.");
            return string.Empty; 
        }

        return $"ch_mock_{Guid.NewGuid():N}";
    }
}