namespace MazadZone.Application.Features.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).ValidateEmail(); 

        RuleFor(x => x.Password).ValidatePassword();
    }
}
