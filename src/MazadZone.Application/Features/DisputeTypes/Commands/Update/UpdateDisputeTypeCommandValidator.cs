namespace MazadZone.Features.DisputeTypes.Commands.Update;

using FluentValidation;
using MazadZone.Application.Common.Validation;

public sealed class UpdateDisputeTypeCommandValidator : AbstractValidator<UpdateDisputeTypeCommand>
{
    public UpdateDisputeTypeCommandValidator()
    {
        RuleFor(x => x.DisputeTypeId).MustBeValidDisputeTypeId();
        RuleFor(x => x.Name).MustBeValidName();
        RuleFor(x => x.Description).MustBeValidDescription();
    }
}