using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Orders.Commands.ReplyToFeedback;

public class ReplyToFeedbackValidator : AbstractValidator<ReplyToFeedbackCommand>
{
    public ReplyToFeedbackValidator()
    {
        RuleFor(x => x.OrderId).MustBeValidOrderId();

        RuleFor(x => x.ReplyText)
            .NotEmpty()
                .WithMessage("Reply text cannot be empty.")
            .MaximumLength(OrderConstants.MaxCommentLength)
                 .WithMessage($"Reply text must not exceed {OrderConstants.MaxCommentLength} characters.");
    }
}