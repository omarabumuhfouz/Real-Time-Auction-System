namespace MazadZone.Features.DisputeTypes.Queries.GetById;

using FluentValidation;
using MazadZone.Application.Common.Validation;

public sealed class GetDisputeTypeByIdQueryValidator : AbstractValidator<GetDisputeTypeByIdQuery>
{
    public GetDisputeTypeByIdQueryValidator()
    {
        RuleFor(x => x.DisputeTypeId).MustBeValidDisputeTypeId();
    }
}