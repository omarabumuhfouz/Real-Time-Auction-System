using MazadZone.Application.Features.Users.Commands.Suspend;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.Suspend;

public class SuspendUserCommandValidatorTests
{
    private readonly SuspendUserCommandValidator _validator;

    public SuspendUserCommandValidatorTests()
    {
        _validator = new SuspendUserCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        // We use a date explicitly in the future to pass the GreaterThan rule
        var validFutureDate = DateTime.UtcNow.AddDays(7);
        var command = new SuspendUserCommand(UserId.New(), "Valid Reason", validFutureDate);

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
        var futureDate = DateTime.UtcNow.AddDays(7);
        var command = new SuspendUserCommand(emptyId, "Valid Reason", futureDate);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        
        // Ensure the date rule didn't falsely trigger
        result.ShouldNotHaveValidationErrorFor(x => x.Until); 
    }

    [Fact]
    public void Validate_UntilDateIsInPast_FailsValidation()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1); // Clearly in the past
        var command = CreateCommand(userId: UserId.New(), reason: "Valid Reason", suspendUntil: pastDate);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Until);

        // Ensure the ID rule didn't falsely trigger
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    private SuspendUserCommand CreateCommand(UserId? userId = null, string? reason = null, DateTime? suspendUntil = null)
    {
        return new SuspendUserCommand(
            userId ?? UserId.New(),
            reason ?? "Violation of terms of service.",
            suspendUntil ?? DateTime.UtcNow.AddDays(7));
    }
}