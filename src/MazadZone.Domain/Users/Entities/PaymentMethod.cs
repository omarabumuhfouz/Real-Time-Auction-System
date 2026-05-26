using MazadZone.Domain.Users.Enums;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.ValueObjects;
using MazadZone.Domain.Primitives;
using MazadZone.Domain.Primitives.Results;

namespace MazadZone.Domain.Users.Entities;

public sealed class PaymentMethod : Entity<PaymentMethodId>, IAuditableEntity
{
    private PaymentMethod() { }

    private PaymentMethod(
        PaymentMethodId id,
        UserId userId,
        string last4Digits,
        int expiryMonth,
        int expiryYear,
        string cardholderName,
        CardBrand brand,
        string gatewayToken,
        bool isDefault) : base(id)
    {
        UserId = userId;
        Last4Digits = last4Digits;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        CardholderName = cardholderName;
        Brand = brand;
        GatewayToken = gatewayToken;
        IsDefault = isDefault;
    }

    // --- Properties ---

    /// <summary>The user that owns this payment method.</summary>
    public UserId UserId { get; private set; }

    /// <summary>Last 4 digits of the card number for display purposes (never raw PAN).</summary>
    public string Last4Digits { get; private set; } = string.Empty;

    public int ExpiryMonth { get; private set; }
    public int ExpiryYear { get; private set; }

    public string CardholderName { get; private set; } = string.Empty;

    public CardBrand Brand { get; private set; }

    /// <summary>The gateway-issued token (e.g., Stripe pm_xxx) representing this card vault entry.</summary>
    public string GatewayToken { get; private set; } = string.Empty;

    /// <summary>Whether this is the user's default payment method.</summary>
    public bool IsDefault { get; private set; }

    // IAuditableEntity
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // --- Factory ---

    public static Result<PaymentMethod> Create(
        UserId userId,
        string last4Digits,
        int expiryMonth,
        int expiryYear,
        string cardholderName,
        CardBrand brand,
        string gatewayToken,
        bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(last4Digits) || last4Digits.Length != PaymentMethodConstants.Last4DigitsLength)
            return Error.Validation("PaymentMethod.InvalidLast4", "Last 4 digits must be exactly 4 characters.");

        if (expiryMonth is < 1 or > 12)
            return Error.Validation("PaymentMethod.InvalidExpiryMonth", "Expiry month must be between 1 and 12.");

        if (expiryYear < DateTime.UtcNow.Year)
            return Error.Validation("PaymentMethod.InvalidExpiryYear", "Expiry year cannot be in the past.");

        if (string.IsNullOrWhiteSpace(cardholderName))
            return Error.Validation("PaymentMethod.InvalidCardholderName", "Cardholder name is required.");

        if (string.IsNullOrWhiteSpace(gatewayToken))
            return Error.Validation("PaymentMethod.InvalidGatewayToken", "Gateway token is required.");

        return new PaymentMethod(
            PaymentMethodId.New(),
            userId,
            last4Digits.Trim(),
            expiryMonth,
            expiryYear,
            cardholderName.Trim(),
            brand,
            gatewayToken.Trim(),
            isDefault);
    }

    // --- Business Behaviours ---

    /// <summary>Mark this method as the user's default.</summary>
    public Result SetAsDefault()
    {
        if (IsDefault)
            return PaymentMethodErrors.AlreadyDefault;

        IsDefault = true;
        return Result.Success();
    }

    /// <summary>Remove the default flag (called on the previously-default method when a new one is promoted).</summary>
    public void UnsetDefault() => IsDefault = false;
}
