namespace MazadZone.Application.Users.Commands.CreateAdminUser;

public class CreateAdminUserCommandValidator : AbstractValidator<CreateAdminUserCommand>
{
    public CreateAdminUserCommandValidator()
    {
        RuleFor(x => x.Email).MustBeValidEmail();
        RuleFor(x => x.Password).MustBeValidPassword();
        RuleFor(x => x.PhoneNumber).MustBeValiePhoneNumber();

        RuleFor(x => x.FirstName).MustBeValidName("First Name");
        RuleFor(x => x.LastName).MustBeValidName("Last Name");
        RuleFor(x => x.SecondName).MustBeValidName("Second Name");
        RuleFor(x => x.ThirdName).MustBeValidName("Third Name");
    }
}