namespace MazadZone.Application.Features.ChatAgent.Commands.SendChatMessage;

/// <summary>
/// Validates the SendChatMessageCommand before it reaches the handler.
/// </summary>
public sealed class SendChatMessageCommandValidator : AbstractValidator<SendChatMessageCommand>
{
    public SendChatMessageCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.UserMessage)
            .NotEmpty()
            .WithMessage("Message cannot be empty.")
            .MaximumLength(500)
            .WithMessage("Message cannot exceed 500 characters.");
    }
}
