namespace MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;

public class GetUserGrowthTrendsQueryValidator : AbstractValidator<GetUserGrowthTrendsQuery>
{
    public GetUserGrowthTrendsQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before the end date.");
            
        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("End date cannot be in the future.");

        RuleFor(x => x.Period)
            .IsEnumName(typeof(ChartGroupingPeriod), caseSensitive: false)
            .WithMessage("Invalid grouping period. Must be: Day, Week, Month, Quarter, or Year.");
    }
}