using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Users.Entities;

namespace MazadZone.Domain.Users.Errors;

public static class PaymentMethodErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "PaymentMethod.NotFound",
        "The payment method was not found.");

    public static readonly Error MaxPaymentMethodsReached = Error.Conflict(
        "PaymentMethod.MaxPaymentMethodsReached",
        $"A user cannot have more than {PaymentMethodConstants.MaxPerUser} saved payment methods.");

    public static readonly Error AlreadyDefault = Error.Conflict(
        "PaymentMethod.AlreadyDefault",
        "This payment method is already set as the default.");
}
