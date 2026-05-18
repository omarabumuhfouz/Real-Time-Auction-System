using MazadZone.Application.Features.Users.Commands.Activate;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.Activate;

public class ActivateUserCommandValidatorTests
{
    private readonly ActivateUserCommandValidator _validator;

    public ActivateUserCommandValidatorTests()
    {
        _validator = new ActivateUserCommandValidator();
    }

    [Fact]
    public void Validate_UserIdIsValid_PassesValidation()
    {
        // Arrange - Generate a valid, non-empty domain ID instance
        var command = new ActivateUserCommand(UserId.New());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_UserIdIsEmpty_FailsValidation()
    {
        // Arrange - Create an ID wrapping an empty Guid to trip the validation rules
        // Note: If your Vogen configuration throws on instantiation for Guid.Empty,
        // use your project's fallback instantiation mechanism here instead.
        var emptyId = UserId.From(Guid.Empty);
        var command = new ActivateUserCommand(emptyId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}