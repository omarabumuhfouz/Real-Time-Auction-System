using MazadZone.Domain.Users.Enums;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Users.Commands.AddPaymentMethod;

public sealed record AddPaymentMethodResponse(
    PaymentMethodId Id,
    string Last4Digits,
    int ExpiryMonth,
    int ExpiryYear,
    string CardholderName,
    CardBrand Brand,
    bool IsDefault,
    DateTime CreatedOnUtc);
