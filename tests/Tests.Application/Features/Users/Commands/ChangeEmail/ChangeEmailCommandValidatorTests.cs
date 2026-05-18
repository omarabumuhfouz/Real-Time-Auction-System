using AuthService.Application.Features.Users.Commands.ChangeEmail;
using MazadZone.Application.Features.Users.Commands.ChangeEmail;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.ChangeEmail;

public class ChangeEmailCommandValidatorTests
{
    private readonly ChangeEmailCommandValidator _validator;

    public ChangeEmailCommandValidatorTests()
    {
        _validator = new ChangeEmailCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new ChangeEmailCommand(UserId.New(), "valid.email@mazadzone.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_UserIdIsEmpty_FailsValidation()
    {
        // Arrange
        var emptyId = UserId.From(Guid.Empty);
        var command = new ChangeEmailCommand(emptyId, "valid.email@mazadzone.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        
        // Ensure the valid email didn't falsely trigger an error
        result.ShouldNotHaveValidationErrorFor(x => x.NewEmail);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("plainaddress")] // Missing @ and domain
    [InlineData("@missingusername.com")] // Missing username
    public void Validate_EmailIsInvalid_FailsValidation(string? invalidEmail)
    {
        // Arrange
        var command = new ChangeEmailCommand(UserId.New(), invalidEmail!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.NewEmail);
        
        // Ensure the valid UserId didn't falsely trigger an error
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }
}