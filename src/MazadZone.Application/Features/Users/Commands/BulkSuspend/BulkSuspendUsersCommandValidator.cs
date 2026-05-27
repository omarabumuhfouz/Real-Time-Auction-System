using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Users.Commands.BulkSuspend;

public class BulkSuspendUsersCommandValidator : AbstractValidator<BulkSuspendUsersCommand>
{
    public BulkSuspendUsersCommandValidator()
    {
        RuleFor(x => x.UserIds)
            .NotEmpty().WithMessage("At least one user must be selected for suspension.");

        RuleForEach(x => x.UserIds).MustBeValidUserId();

        RuleFor(x => x.Reason).MustBeValidReason();
        
        RuleFor(x => x.Until).GreaterThan(DateTime.UtcNow);
    }
}