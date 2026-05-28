namespace Tests.Application.Features.DisputeTypes.Commands.Update;

using System;
using FluentValidation.TestHelper;
using MazadZone.Features.DisputeTypes.Commands.Update;
using Xunit;

public class UpdateDisputeTypeCommandValidatorTests
{
    private readonly UpdateDisputeTypeCommandValidator _validator;

    public UpdateDisputeTypeCommandValidatorTests()
    {
        _validator = new UpdateDisputeTypeCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = new UpdateDisputeTypeCommand(
            DisputeTypeId.New(), 
            "Updated Name", 
            "Updated Description that meets all criteria.");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDisputeTypeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        // We use a valid name and description so ONLY the ID fails
        var command = new UpdateDisputeTypeCommand(
            DisputeTypeId.Empty, 
            "Valid Name", 
            "Valid Description");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisputeTypeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenNameIsInvalid_ShouldHaveValidationError(string? invalidName)
    {
        // Arrange
        var command = new UpdateDisputeTypeCommand(
            DisputeTypeId.New(), 
            invalidName!, 
            "Valid Description");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.DisputeTypeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenDescriptionIsInvalid_ShouldHaveValidationError(string? invalidDescription)
    {
        // Arrange
        var command = new UpdateDisputeTypeCommand(
            DisputeTypeId.New(), 
            "Valid Name", 
            invalidDescription!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.DisputeTypeId);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}