using MazadZone.Application.Features.Users.Commands.Ban;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.Ban;

public class BanUserCommandValidatorTests
{
    private readonly BanUserCommandValidator _validator;

    public BanUserCommandValidatorTests()
    {
        _validator = new BanUserCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var validId = UserId.New();
        var validReason = "Violation of terms of service: multiple unpaid bids.";
        var command = new BanUserCommand(validId, validReason);

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
        var command = new BanUserCommand(emptyId, "Valid reason text");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        
        // Ensure the Reason rule didn't falsely fail
        result.ShouldNotHaveValidationErrorFor(x => x.Reason); 
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReasonIsEmpty_FailsValidation(string? invalidReason)
    {
        // Arrange
        var command = new BanUserCommand(UserId.New(), invalidReason!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Reason);
        
        // Ensure the UserId rule didn't falsely fail
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }
}