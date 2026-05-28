namespace MazadZone.Application.Features.Disputes.Queries.ExportSelectedDisputes;

using FluentValidation;

public class ExportSelectedDisputesQueryValidator : AbstractValidator<ExportSelectedDisputesQuery>
{
    public ExportSelectedDisputesQueryValidator()
    {
        RuleFor(x => x.SelectedDisputeIds)
            .NotEmpty()
            .WithMessage("At least one dispute ID must be provided for export.");
    }
}