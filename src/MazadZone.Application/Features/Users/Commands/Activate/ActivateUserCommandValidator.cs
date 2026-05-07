using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Users.Commands.Activate;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(UserErrorCodes.IdRequired);
    }
}