using FluentValidation;
using MazadZone.Domain.Bidders;

namespace MazadZone.Application.Features.Bidders.Queries.GetMyBids;

public sealed class GetMyBidsValidator : AbstractValidator<GetMyBidsQuery>
{
    private static readonly string[] AllowedTabs = new[] { "all", "leading", "outbid", "ended" };
    private static readonly string[] AllowedSortBy = new[] { "CreationDate", "StartTime", "EndTime", "CurrentBidAmount", "YourBidAmount" };

    public GetMyBidsValidator()
    {
        RuleFor(x => x.BidderId)
            .NotEqual(BidderId.Empty)
            .WithMessage("A valid bidder id is required.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage("Search term must not exceed 200 characters.");

        RuleFor(x => x.Tab)
            .Must(tab => !string.IsNullOrEmpty(tab) && AllowedTabs.Contains(tab.ToLowerInvariant()))
            .WithMessage("Tab must be one of: all, leading, outbid, ended.");

        RuleFor(x => x.SortBy)
            .Must(sortBy => !string.IsNullOrEmpty(sortBy) && AllowedSortBy.Contains(sortBy))
            .WithMessage("Sort by must be one of: CreationDate, StartTime, EndTime, CurrentBidAmount, YourBidAmount.");

        RuleFor(x => x.SortDirection)
            .Must(direction => direction == "asc" || direction == "desc")
            .WithMessage("Sort direction must be 'asc' or 'desc'.");
    }
}
