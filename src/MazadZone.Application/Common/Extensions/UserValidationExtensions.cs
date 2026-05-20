using System.Text.RegularExpressions;
using AuthService.Domain.Constants;

namespace MazadZone.Application.Common.Extensions;

public static class UserValidationExtensions
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex NameRegex = new(
        @"^[a-zA-Z\u0600-\u06FF\s'-]+$", // Supports English, Arabic, spaces, hyphens, and apostrophes
        RegexOptions.Compiled);

    // private static readonly Regex PhoneRegex = new(
    //     @"^\+?[1-9]\d{1,14}$", // International E.164 format
    //     RegexOptions.Compiled);

    public static IRuleBuilderOptions<T, string> ValidateEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email is required.")
            .Matches(EmailRegex).WithMessage("The email format is not valid.");
    }

    public static IRuleBuilderOptions<T, string> ValidatePassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
        .NotNull().WithMessage("Password is required.")
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(UserConstants.PasswordMinLength).WithMessage("Password must be at least 8 characters.")
            .Must(password => password.Any(char.IsUpper)).WithMessage("Password must contain at least one uppercase letter.")
            .Must(password => password.Any(char.IsLower)).WithMessage("Password must contain at least one lowercase letter.")
            .Must(password => password.Any(char.IsDigit)).WithMessage("Password must contain at least one digit.")
            .Must(password => password.Any(ch => !char.IsLetterOrDigit(ch))).WithMessage("Password must contain at least one special character.");
    }

    public static IRuleBuilderOptions<T, string> ValidateFirstName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(UserConstants.NameMaxLength).WithMessage("First name cannot exceed 50 characters.")
            .Matches(NameRegex).WithMessage("First name contains invalid characters.");
    }

    public static IRuleBuilderOptions<T, string> ValidateSecondName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(UserConstants.NameMaxLength).WithMessage("Second name cannot exceed 50 characters.")
            .Matches(NameRegex).WithMessage("Second name contains invalid characters.");
    }

    public static IRuleBuilderOptions<T, string> ValidateThirdName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(UserConstants.NameMaxLength).WithMessage("Third name cannot exceed 50 characters.")
            .Matches(NameRegex).WithMessage("Third name contains invalid characters.");
    }

    public static IRuleBuilderOptions<T, string> ValidateLastName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(UserConstants.NameMaxLength).WithMessage("Last name cannot exceed 50 characters.")
            .Matches(NameRegex).WithMessage("Last name contains invalid characters.");
    }

    public static IRuleBuilderOptions<T, string> ValidatePhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Phone number is required.")
            .Length(UserConstants.PhoneNumberLength).WithMessage($"Phone number must be exactly {UserConstants.PhoneNumberLength} digits.");
    }
}