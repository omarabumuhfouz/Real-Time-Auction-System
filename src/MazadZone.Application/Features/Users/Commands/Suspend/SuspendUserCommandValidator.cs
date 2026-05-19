using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Users.Commands.Suspend;

public class SuspendUserCommandValidator : AbstractValidator<SuspendUserCommand>
{
    public SuspendUserCommandValidator()
    {
        RuleFor(x => x.UserId).MustBeValidUserId();
        RuleFor(x => x.Until).GreaterThan(DateTime.UtcNow);
    }
}