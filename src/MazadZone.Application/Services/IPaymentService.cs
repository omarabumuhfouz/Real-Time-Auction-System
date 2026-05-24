using MazadZone.Domain.Auctions;
using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Domain.ValueObjects;
using MzadZone.Domain.Payments;

namespace MazadZone.Application.Services;
public interface IPaymentService : ITransientService
{
    Task<string> AuthorizeAsync(Guid userId, Guid auctionId, string methodId, Money depositAmount, CancellationToken cancellationToken);
    Task UnAuthorizeAsync(string gatewayAuthHoldId, CancellationToken cancellationToken);
    Task<string?> CaptureHoldedAmountAsync(string gatewayAuthHoldId, CancellationToken cancellationToken);
    Task<string?> ChargeAmountAsync(Guid userId, Money Amount, string paymentMethodId, CancellationToken cancellationToken);
}

public record AuthorizationRequest(BidId BidId, Money Amount);