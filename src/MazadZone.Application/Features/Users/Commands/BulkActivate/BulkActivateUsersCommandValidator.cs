using MazadZone.Application.Common.Validation;
using Microsoft.IdentityModel.Tokens;

namespace MazadZone.Application.Features.Users.Commands.BulkActivate;

public class BulkActivateUsersCommandValidator : AbstractValidator<BulkActivateUsersCommand>
{
    public BulkActivateUsersCommandValidator()
    {
        RuleFor(x => x.UserIds)
           .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("You must provide at least one user ID to activate.")
            .Must(ids => ids.Count <= 100).WithMessage("You cannot activate more than 100 users at once.");

        RuleForEach(x => x.UserIds)
            .MustBeValidUserId(); 
    }
}