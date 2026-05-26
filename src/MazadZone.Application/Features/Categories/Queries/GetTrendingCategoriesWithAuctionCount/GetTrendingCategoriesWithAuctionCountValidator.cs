namespace MazadZone.Application.Features.Categories.Queries.GetTrendingCategoriesWithAuctionCount;

public class GetTrendingCategoriesWithAuctionCountValidator : AbstractValidator<GetTrendingCategoriesWithAuctionCountQuery>
{
    public GetTrendingCategoriesWithAuctionCountValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Limit cannot exceed 100 entries to protect database throughput.");
    }
}