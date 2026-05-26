using MazadZone.Application.Common.Validators;

namespace MazadZone.Application.Features.Bidders.Commands.Register;

public class RegisterBidderValidator : AbstractValidator<RegisterBidderCommand>
{
    public RegisterBidderValidator()
    {
        RuleFor(x => x.Email).MustBeValidEmail();

        RuleFor(x => x.Password).MustBeValidPassword();

        RuleFor(x => x.PhoneNumber).MustBeValiePhoneNumber();

        RuleFor(x => x.FirstName).MustBeValidName("First Name");
        RuleFor(x => x.LastName).MustBeValidName("Last Name");
        RuleFor(x => x.SecondName).MustBeValidName("Second Name");
        RuleFor(x => x.ThirdName).MustBeValidName("Third Name");

        RuleFor(x => x.Address)
            .NotNull()
            .SetValidator(new AddressDtoValidator());
    }
}