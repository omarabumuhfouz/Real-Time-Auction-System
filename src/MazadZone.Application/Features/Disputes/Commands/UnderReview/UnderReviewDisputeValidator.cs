using MazadZone.Application.Common.Validation;
using MazadZone.Application.Features.Orders.Commands.ResolveDispute;

namespace MazadZone.Application.Orders.ResolveDispute;

public class UnderReviewDisputeValidator : AbstractValidator<UnderReviewDisputeCommand>
{
    public UnderReviewDisputeValidator()
    {
        RuleFor(d => d.DisputeId).MustBeValidDisputeId();
    }
}