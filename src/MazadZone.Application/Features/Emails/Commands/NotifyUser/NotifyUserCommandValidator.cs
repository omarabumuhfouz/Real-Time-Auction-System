namespace MazadZone.Application.Features.Emails.Commands.NotifyUser;

public sealed class NotifyUserCommandValidator : AbstractValidator<NotifyUserCommand>
{
    public NotifyUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Email title is required.")
            .MaximumLength(150).WithMessage("Email title must not exceed 150 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Email message body is required.")
            .MinimumLength(5).WithMessage("Message must be at least 5 characters long.")
            .MaximumLength(2000).WithMessage("Message must not exceed 2000 characters.");
    }
}