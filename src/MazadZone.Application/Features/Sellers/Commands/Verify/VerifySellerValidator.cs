using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Sellers.Commands.Verify;

public sealed class VerifySellerValidator : AbstractValidator<VerifySellerCommand>
{
    public VerifySellerValidator()
    {
        RuleFor(x => x.SellerId).MustBeValidUserId();
    }
}