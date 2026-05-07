using MazadZone.Application.Features.Sellers.Validation;

namespace MazadZone.Application.Features.Sellers.Commands.UpdateBankDetails;

public sealed class UpdateBankDetailsValidator : AbstractValidator<UpdateBankDetailsCommand>
{
    public UpdateBankDetailsValidator()
    {
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.NewAccountNumber).MustBeValidBankAccount();
    }
}