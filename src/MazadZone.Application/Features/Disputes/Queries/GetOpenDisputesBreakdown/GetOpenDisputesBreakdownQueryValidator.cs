namespace MazadZone.Application.Features.Disputes.Queries.GetOpenDisputesBreakdown;

public class GetOpenDisputesBreakdownQueryValidator : AbstractValidator<GetOpenDisputesBreakdownQuery>
{
    public GetOpenDisputesBreakdownQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before the end date.");
            
        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("End date cannot be in the future.");
    }
}