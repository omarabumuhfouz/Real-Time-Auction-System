namespace Tests.Application.Features.DisputeTypes.Commands.Restore;

using System;
using FluentValidation.TestHelper;
using MazadZone.Features.DisputeTypes.Commands.Restore;
using Xunit;

public class RestoreDisputeTypeCommandValidatorTests
{
    private readonly RestoreDisputeTypeCommandValidator _validator;

    public RestoreDisputeTypeCommandValidatorTests()
    {
        _validator = new RestoreDisputeTypeCommandValidator();
    }

    [Fact]
    public void Validate_WhenDisputeTypeIdIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = new RestoreDisputeTypeCommand(DisputeTypeId.New());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDisputeTypeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        // Assuming MustBeValidDisputeTypeId() rejects Guid.Empty
        var command = new RestoreDisputeTypeCommand(DisputeTypeId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisputeTypeId);
    }
}