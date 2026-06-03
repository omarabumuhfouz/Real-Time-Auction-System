namespace MazadZone.Application.Features.Users.Queries.GetUserTrustStats;

public class GetUserTrustStatsQueryValidator : AbstractValidator<GetUserTrustStatsQuery>
{
    public GetUserTrustStatsQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before the end date.");
            
        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("End date cannot be in the future.");
    }
}