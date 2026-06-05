using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Users.Queries.GetPaymentMethods;

public class GetPaymentMethodsQueryValidator : AbstractValidator<GetPaymentMethodsQuery>
{
    public GetPaymentMethodsQueryValidator()
    {
        RuleFor(x => x.UserId).MustBeValidUserId();
    }
}
