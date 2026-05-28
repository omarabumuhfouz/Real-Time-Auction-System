namespace Tests.Application.Features.DisputeTypes.Commands.Create;

using FluentValidation.TestHelper;
using MazadZone.Features.DisputeTypes.Commands.Create;
using Xunit;

public class CreateDisputeTypeCommandValidatorTests
{
    private readonly CreateDisputeTypeCommandValidator _validator;

    public CreateDisputeTypeCommandValidatorTests()
    {
        _validator = new CreateDisputeTypeCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = DisputeTypeHelper.CreateValidCreateCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    // [InlineData("ThisNameIsWayTooLongAndExceedsTheMaximumAllowedCharactersForADisputeTypeName...")] 
    public void Validate_WhenNameIsInvalid_ShouldHaveValidationError(string? invalidName)
    {
        // Arrange
        var command = DisputeTypeHelper.CreateValidCreateCommand() with { Name = invalidName! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    // Add another InlineData here if MustBeValidDescription() checks for max length
    public void Validate_WhenDescriptionIsInvalid_ShouldHaveValidationError(string? invalidDescription)
    {
        // Arrange
        var command = DisputeTypeHelper.CreateValidCreateCommand() with { Description = invalidDescription! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}