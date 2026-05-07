using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Sellers.Validation;

public static class SellerValidationExtensions
{
    

    public static IRuleBuilderOptions<T, string> MustBeValidBankAccount<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(SellerConstants.MaxBankAccountNumber)
            .WithMessage($"Bank account number cannot exceed {SellerConstants.MaxBankAccountNumber} characters.");
    }

    public static IRuleBuilderOptions<T, string?> MustBeValidTaxId<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(SellerConstants.MaxTaxId)
            .WithMessage("Tax Identification Number is too long.");
    }
}