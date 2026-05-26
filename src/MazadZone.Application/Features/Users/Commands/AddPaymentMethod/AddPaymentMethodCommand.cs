using MazadZone.Domain.Users.Enums;

namespace MazadZone.Application.Features.Users.Commands.AddPaymentMethod;

/// <summary>
/// Command to add a new payment method for a user.
/// UserId is resolved from the authenticated JWT claims in the API layer, not from the request body.
/// </summary>
public sealed record AddPaymentMethodCommand(
    UserId UserId,
    string Last4Digits,
    int ExpiryMonth,
    int ExpiryYear,
    string CardholderName,
    CardBrand Brand,
    string GatewayToken,
    bool IsDefault) : ICommand<AddPaymentMethodResponse>;
