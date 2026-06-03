namespace Tests.Application.Features.Users.Commands.AddPaymentMethod;

using System;
using FluentValidation.TestHelper;
using MazadZone.Application.Features.Users.Commands.AddPaymentMethod;
using MazadZone.Domain.Users.Enums;
using Xunit;

public class AddPaymentMethodCommandValidatorTests
{
    private readonly AddPaymentMethodCommandValidator _validator;

    public AddPaymentMethodCommandValidatorTests()
    {
        _validator = new AddPaymentMethodCommandValidator();
    }

    // --- Helper Method ---
    private AddPaymentMethodCommand CreateValidCommand()
    {
        return new AddPaymentMethodCommand(
            UserId.New(), // Or whatever generates a valid ID
            "4242",
            12,
            DateTime.UtcNow.Year + 2,
            "John Doe",
            CardBrand.Visa,
            "tok_valid_gateway_token_123",
            true);
    }

    // --- Happy Path ---
    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        var command = CreateValidCommand();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- UserId Validation ---
    [Fact]
    public void Validate_WhenUserIdIsInvalid_ShouldHaveValidationError()
    {
        // Assuming UserId.Empty exists or new UserId(Guid.Empty) triggers your MustBeValidUserId()
        var command = CreateValidCommand() with { UserId = UserId.Empty }; // Adjust based on your strongly-typed ID setup

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // --- Last4Digits Validation ---
    [Theory]
    [InlineData("", "Last 4 digits are required.")]
    [InlineData("   ", "Last 4 digits are required.")] // Whitespace is treated as empty by .NotEmpty()
    [InlineData("123", "Last 4 digits must be exactly 4 characters.")] // Too short
    [InlineData("12345", "Last 4 digits must be exactly 4 characters.")] // Too long
    [InlineData("12A4", "Last 4 digits must contain only numbers.")] // Contains letters
    [InlineData("12-4", "Last 4 digits must contain only numbers.")] // Contains symbols
    public void Validate_WhenLast4DigitsIsInvalid_ShouldHaveValidationError(string invalidDigits, string expectedMessage)
    {
        var command = CreateValidCommand() with { Last4Digits = invalidDigits };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Last4Digits)
              .WithErrorMessage(expectedMessage);
    }

    // --- ExpiryMonth Validation ---
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(13)]
    [InlineData(99)]
    public void Validate_WhenExpiryMonthIsOutOfBounds_ShouldHaveValidationError(int invalidMonth)
    {
        var command = CreateValidCommand() with { ExpiryMonth = invalidMonth };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpiryMonth)
              .WithErrorMessage("Expiry month must be between 1 and 12.");
    }

    // --- ExpiryYear Validation ---
    [Fact]
    public void Validate_WhenExpiryYearIsInPast_ShouldHaveValidationError()
    {
        var pastYear = DateTime.UtcNow.Year - 1;
        var command = CreateValidCommand() with { ExpiryYear = pastYear };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpiryYear)
              .WithErrorMessage("Expiry year cannot be in the past.");
    }

    // --- CardholderName Validation ---
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenCardholderNameIsEmpty_ShouldHaveValidationError(string? invalidName)
    {
        var command = CreateValidCommand() with { CardholderName = invalidName! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CardholderName)
              .WithErrorMessage("Cardholder name is required.");
    }

    [Fact]
    public void Validate_WhenCardholderNameExceedsMaxLength_ShouldHaveValidationError()
    {
        // 101 characters long
        var tooLongName = new string('A', 101);
        var command = CreateValidCommand() with { CardholderName = tooLongName };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CardholderName)
              .WithErrorMessage("Cardholder name cannot exceed 100 characters.");
    }

    // --- GatewayToken Validation ---
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenGatewayTokenIsEmpty_ShouldHaveValidationError(string? invalidToken)
    {
        var command = CreateValidCommand() with { GatewayToken = invalidToken! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GatewayToken)
              .WithErrorMessage("Gateway token is required.");
    }

    [Fact]
    public void Validate_WhenGatewayTokenExceedsMaxLength_ShouldHaveValidationError()
    {
        // 256 characters long
        var tooLongToken = new string('A', 256);
        var command = CreateValidCommand() with { GatewayToken = tooLongToken };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GatewayToken)
              .WithErrorMessage("Gateway token is too long.");
    }

    // --- Brand Validation ---
    [Fact]
    public void Validate_WhenBrandIsInvalid_ShouldHaveValidationError()
    {
        // Force-cast an invalid integer to the enum
        var invalidBrand = (CardBrand)999;
        var command = CreateValidCommand() with { Brand = invalidBrand };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Brand)
              .WithErrorMessage("Invalid card brand.");
    }
}