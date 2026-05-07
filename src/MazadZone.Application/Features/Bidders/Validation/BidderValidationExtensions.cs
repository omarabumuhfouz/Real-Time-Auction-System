using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Bidders.Validation;

internal static class BidderValidationExtensions
{
    internal static IRuleBuilderOptions<T, BidderId> ValidateBidderId<T>(this IRuleBuilder<T, BidderId> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Bidder identifier is required.");
    }
}