using MazadZone.Application.Common.Validation;

namespace MazadZone.Application.Features.Users.Commands.AddPaymentMethod;

public sealed class AddPaymentMethodCommandValidator : AbstractValidator<AddPaymentMethodCommand>
{
    public AddPaymentMethodCommandValidator()
    {
        RuleFor(x => x.UserId)
            .MustBeValidUserId();

        RuleFor(x => x.Last4Digits)
            .NotEmpty().WithMessage("Last 4 digits are required.")
            .Length(4).WithMessage("Last 4 digits must be exactly 4 characters.")
            .Matches(@"^\d{4}$").WithMessage("Last 4 digits must contain only numbers.");

        RuleFor(x => x.ExpiryMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Expiry month must be between 1 and 12.");

        RuleFor(x => x.ExpiryYear)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage("Expiry year cannot be in the past.");

        RuleFor(x => x.CardholderName)
            .NotEmpty().WithMessage("Cardholder name is required.")
            .MaximumLength(100).WithMessage("Cardholder name cannot exceed 100 characters.");

        RuleFor(x => x.GatewayToken)
            .NotEmpty().WithMessage("Gateway token is required.")
            .MaximumLength(255).WithMessage("Gateway token is too long.");

        RuleFor(x => x.Brand)
            .IsInEnum().WithMessage("Invalid card brand.");
    }
}
