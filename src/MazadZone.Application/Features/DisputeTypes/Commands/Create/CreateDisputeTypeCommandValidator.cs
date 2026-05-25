namespace MazadZone.Features.DisputeTypes.Commands.Create;

using FluentValidation;

public sealed class CreateDisputeTypeCommandValidator : AbstractValidator<CreateDisputeTypeCommand>
{
    public CreateDisputeTypeCommandValidator()
    {
        RuleFor(x => x.Name).MustBeValidName();
        RuleFor(x => x.Description).MustBeValidDescription();
    }
}