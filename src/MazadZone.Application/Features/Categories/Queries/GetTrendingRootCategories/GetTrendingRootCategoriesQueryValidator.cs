namespace MazadZone.Application.Features.Categories.Queries.GetTrendingRootCategories;

public class GetTrendingRootCategoriesQueryValidator : AbstractValidator<GetTrendingRootCategoriesQuery>
{
    public GetTrendingRootCategoriesQueryValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithMessage("Limit must be at least 1.")
            .LessThanOrEqualTo(50)
            .WithMessage("Limit cannot exceed 50 to maintain dashboard performance.");
    }
}