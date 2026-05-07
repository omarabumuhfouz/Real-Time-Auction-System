using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Common.Validation;

public static class GlobalValidationExtensions
{
    public static IRuleBuilderOptions<T, BidderId> MustBeValidBidderId<T>(this IRuleBuilder<T, BidderId> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Bidder ID is required and cannot be empty.");
    }

    public static IRuleBuilderOptions<T, SellerId> MustBeValidSellerId<T>(this IRuleBuilder<T, SellerId> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Seller identifier is required.");
    }
    

    public static IRuleBuilderOptions<T, UserId> MustBeValidUserId<T>(this IRuleBuilder<T, UserId> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("User identifier is required.");
    }
}