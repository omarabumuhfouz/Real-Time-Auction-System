namespace MazadZone.Domain.Users.Entities;

public static class PaymentMethodConstants
{
    /// <summary>Maximum number of saved payment methods per user.</summary>
    public const int MaxPerUser = 5;

    public const int Last4DigitsLength = 4;
    public const int CardholderNameMaxLength = 100;
    public const int GatewayTokenMaxLength = 255;
}
