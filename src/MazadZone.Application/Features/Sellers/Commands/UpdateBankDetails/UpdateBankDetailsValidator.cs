using MazadZone.Application.Common.Validation;
using MazadZone.Application.Features.Sellers.Validation;

namespace MazadZone.Application.Features.Sellers.Commands.UpdateBankDetails;

public sealed class UpdateBankDetailsValidator : AbstractValidator<UpdateBankDetailsCommand>
{
    public UpdateBankDetailsValidator()
    {
        RuleFor(x => x.SellerId).MustBeValidUserId();
        RuleFor(x => x.NewAccountNumber).MustBeValidBankAccount();
    }
}