using MazadZone.Domain.Users.Enums;

namespace MazadZone.Application.Features.Users.Queries.GetPaymentMethods;

public sealed record PaymentMethodResponse(
    Guid Id,
    string Last4Digits,
    int ExpiryMonth,
    int ExpiryYear,
    string CardholderName,
    CardBrand Brand,
    bool IsDefault,
    DateTime CreatedOnUtc);
