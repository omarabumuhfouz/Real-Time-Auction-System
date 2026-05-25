using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Bidders.Commands.Verify;

public class VerifyBidderCommandValidator : AbstractValidator<VerifyBidderCommand>
{
    public VerifyBidderCommandValidator()
    {
        RuleFor(x => x.BidderId).MustBeValidUserId();
    }
}