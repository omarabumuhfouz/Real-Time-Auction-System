namespace MazadZone.Features.DisputeTypes.Commands.Restore;

using FluentValidation;
using MazadZone.Application.Common.Validation;

public sealed class RestoreDisputeTypeCommandValidator : AbstractValidator<RestoreDisputeTypeCommand>
{
    public RestoreDisputeTypeCommandValidator()
    {
        RuleFor(x => x.DisputeTypeId).MustBeValidDisputeTypeId();
    }
}