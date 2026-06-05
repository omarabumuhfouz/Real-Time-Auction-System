namespace MazadZone.Application.Features.Users.Queries.GetPaymentMethods;

public record GetPaymentMethodsQuery(UserId UserId) : IQuery<IReadOnlyList<PaymentMethodResponse>>;
