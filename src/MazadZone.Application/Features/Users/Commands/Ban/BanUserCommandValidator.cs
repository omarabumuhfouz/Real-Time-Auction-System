using MazadZone.Domain.Shared;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Users.Commands.Ban;

public class BanUserCommandValidator : AbstractValidator<BanUserCommand>
{
    public BanUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithErrorCode(UserErrorCodes.IdRequired);
        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(SharedConstainst.MaxReasonLength)
            .MinimumLength(SharedConstainst.MinReasonLength);

    }
}