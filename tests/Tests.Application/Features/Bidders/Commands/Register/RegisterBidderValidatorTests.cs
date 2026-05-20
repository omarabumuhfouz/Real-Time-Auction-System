using MazadZone.Application.Features.Bidders.Commands.Register;

namespace Tests.Application.Features.Bidders.Commands.Register;

public class RegisterBidderValidatorTests
{
    private readonly RegisterBidderValidator _validator;

    public RegisterBidderValidatorTests()
    {
        _validator = new RegisterBidderValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = BidderHelper.CreateValidRegisterCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        if (!result.IsValid)
    {
        var errors = string.Join(", ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
        throw new Exception($"Validation failed! Errors: {errors}");
    }
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("plainaddress")] // Missing @ and domain
    // [InlineData("@missingusername.com")]
    public void Validate_EmailIsInvalid_FailsValidation(string? invalidEmail)
    {
        // Arrange
        var command = BidderHelper.CreateValidRegisterCommand() with { Email = invalidEmail! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("short")] // Assuming your ValidatePassword() requires > 8 chars, uppercase, etc.
    [InlineData("nouppercase123!")]
    public void Validate_PasswordIsInvalid_FailsValidation(string? invalidPassword)
    {
        // Arrange
        var command = BidderHelper.CreateValidRegisterCommand() with { Password = invalidPassword! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")] // Too short for a phone number
    public void Validate_PhoneNumberIsInvalid_FailsValidation(string? invalidPhone)
    {
        // Arrange
        var command = BidderHelper.CreateValidRegisterCommand() with { PhoneNumber = invalidPhone! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_FirstNameIsEmpty_FailsValidation(string? invalidName)
    {
        // Arrange
        // Testing one of the name fields to ensure the base extensions are catching empty states
        var command = BidderHelper.CreateValidRegisterCommand() with { FirstName = invalidName! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_AddressIsNull_FailsValidation()
    {
        // Arrange
        // We test the .NotNull() rule directly applied to the Address property
        var command = BidderHelper.CreateValidRegisterCommand() with { Address = null! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

}