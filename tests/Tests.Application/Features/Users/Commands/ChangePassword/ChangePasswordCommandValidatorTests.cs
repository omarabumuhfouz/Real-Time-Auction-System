using AuthService.Application.Features.Users.Commands.ChangePassword;
using MazadZone.Application.Features.Users.Commands.ChangePassword;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator;

    public ChangePasswordCommandValidatorTests()
    {
        _validator = new ChangePasswordCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var validPassword = "StrongP@ssword123!";
        var command = new ChangePasswordCommand(
            UserId.New(), 
            "OldPassword123", 
            validPassword, 
            validPassword); // Confirm matches New

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
        var validPassword = "StrongP@ssword123!";
        var command = new ChangePasswordCommand(
            UserId.From(Guid.Empty), 
            "OldPassword123", 
            validPassword, 
            validPassword);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_CurrentPasswordIsEmpty_FailsValidation(string? invalidCurrentPassword)
    {
        // Arrange
        var validPassword = "StrongP@ssword123!";
        var command = new ChangePasswordCommand(
            UserId.New(), 
            invalidCurrentPassword!, 
            validPassword, 
            validPassword);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("weak")] // Assuming your custom extension catches weak passwords
    public void Validate_NewPasswordIsWeak_FailsValidation(string? invalidNewPassword)
    {
        // Arrange
        var command = new ChangePasswordCommand(
            UserId.New(), 
            "OldPassword123", 
            invalidNewPassword!, 
            invalidNewPassword!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        // Verifies that your .MustBeValidPassword() extension catches the bad input
        result.ShouldHaveValidationErrorFor(x => x.NewPassword); 
    }

    [Fact]
    public void Validate_PasswordsDoNotMatch_ReturnsPasswordsDoNotMatchError()
    {
        // Arrange
        var command = new ChangePasswordCommand(
            UserId.New(), 
            "OldPassword123", 
            "StrongP@ssword123!", 
            "DifferentP@ssword123!"); // Mismatched confirmation

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();

        // Assert exact error code and message generation
        result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword)
            .WithErrorCode(UserErrorCodes.PasswordsDoNotMatch);
    }

    [Fact]
    public void Validate_ConfirmPasswordIsEmpty_ReturnsConfirmPasswordRequiredError()
    {
        // Arrange
        var command = new ChangePasswordCommand(
            UserId.New(), 
            "OldPassword123", 
            "StrongP@ssword123!", 
            string.Empty); // Empty confirmation

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        
        // Assert the specific code assigned to the .NotEmpty() rule
        result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword)
            .WithErrorCode(UserErrorCodes.ConfirmPasswordRequired);
    }
}