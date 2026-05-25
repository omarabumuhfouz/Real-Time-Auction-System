namespace MazadZone.Application.Features.Disputes.Queries.GetById;

using FluentValidation;
using MazadZone.Application.Common.Validation;

public class GetDisputeByIdQueryValidator : AbstractValidator<GetDisputeByIdQuery>
{
    public GetDisputeByIdQueryValidator()
    {
        RuleFor(x => x.DisputeId).MustBeValidDisputeId();
    }
}