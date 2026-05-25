namespace MazadZone.Features.DisputeTypes.Queries.GetById;

using FluentValidation;

public sealed class GetDisputeTypeByIdQueryValidator : AbstractValidator<GetDisputeTypeByIdQuery>
{
    public GetDisputeTypeByIdQueryValidator()
    {
        RuleFor(x => x.DisputeTypeId).NotNull();
    }
}