using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Disputes.Commands.OpenDispute;

public class OpenDisputeValidator : AbstractValidator<OpenDisputeCommand>
{
    public OpenDisputeValidator()
    {
        RuleFor(x => x.OrderId).MustBeValidOrderId();

        RuleFor(x => x.DisputeTypeId).MustBeValidDisputeTypeId();

        RuleFor(x => x.Description).MustBeValidDescription();

        RuleFor(x => x.Title).MustBeValidTitle();

        // RuleFor(x => x.Reason).MustBeValidReason();
    }
}