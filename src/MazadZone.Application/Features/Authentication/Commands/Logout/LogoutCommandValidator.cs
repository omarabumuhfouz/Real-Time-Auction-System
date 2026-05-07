using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Authentication.Commands.Logout;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.UserId).MustBeValidUserId();

        RuleFor(x => x.RefreshToken)
        .NotEmpty()
        .WithMessage("Refresh Token is Required.");
    }
}