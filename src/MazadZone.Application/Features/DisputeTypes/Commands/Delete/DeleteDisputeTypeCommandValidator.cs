namespace MazadZone.Features.DisputeTypes.Commands.Delete;

using FluentValidation;
using MazadZone.Application.Common.Validation;

public sealed class DeleteDisputeTypeCommandValidator : AbstractValidator<DeleteDisputeTypeCommand>
{
    public DeleteDisputeTypeCommandValidator()
    {
        RuleFor(x => x.DisputeTypeId).MustBeValidDisputeTypeId();
    }
}