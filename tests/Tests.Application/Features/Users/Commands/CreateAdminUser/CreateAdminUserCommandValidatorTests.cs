namespace Tests.Application.Users.Commands.CreateAdminUser;

using FluentValidation.TestHelper;
using MazadZone.Application.Users.Commands.CreateAdminUser;
using Xunit;

public class CreateAdminUserCommandValidatorTests
{
    private readonly CreateAdminUserCommandValidator _validator;

    public CreateAdminUserCommandValidatorTests()
    {
        _validator = new CreateAdminUserCommandValidator();
    }

    private CreateAdminUserCommand CreateValidCommand()
    {
        return new CreateAdminUserCommand(
            "admin@example.com",
            "StrongPass123!",
            "1234567890",
            "First",
            "Second",
            "Third",
            "Last"
        );
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    public void Validate_WhenEmailIsInvalid_ShouldHaveValidationError(string? invalidEmail)
    {
        var command = CreateValidCommand() with { Email = invalidEmail };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("weak")] // Assumes your password policy requires stronger passwords
    public void Validate_WhenPasswordIsInvalid_ShouldHaveValidationError(string? invalidPassword)
    {
        var command = CreateValidCommand() with { Password = invalidPassword };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenPhoneNumberIsInvalid_ShouldHaveValidationError(string? invalidPhone)
    {
        var command = CreateValidCommand() with { PhoneNumber = invalidPhone };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenFirstNameIsInvalid_ShouldHaveValidationError(string? invalidName)
    {
        var command = CreateValidCommand() with { FirstName = invalidName };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenSecondNameIsInvalid_ShouldHaveValidationError(string? invalidName)
    {
        var command = CreateValidCommand() with { SecondName = invalidName };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SecondName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenThirdNameIsInvalid_ShouldHaveValidationError(string? invalidName)
    {
        var command = CreateValidCommand() with { ThirdName = invalidName };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ThirdName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenLastNameIsInvalid_ShouldHaveValidationError(string? invalidName)
    {
        var command = CreateValidCommand() with { LastName = invalidName };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}