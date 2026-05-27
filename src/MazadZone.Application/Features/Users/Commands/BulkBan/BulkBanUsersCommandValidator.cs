using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Users.Commands.BulkBan;

public class BulkBanUsersCommandValidator : AbstractValidator<BulkBanUsersCommand>
{
    public BulkBanUsersCommandValidator()
    {
        RuleFor(x => x.UserIds)
            .NotEmpty().WithMessage("At least one user must be selected for banning.");

        // Validates every individual UserId in the collection
        RuleForEach(x => x.UserIds).MustBeValidUserId();

        RuleFor(x => x.Reason).MustBeValidReason();
    }
}