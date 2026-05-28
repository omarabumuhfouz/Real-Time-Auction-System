namespace Tests.Application.Features.DisputeTypes.Commands.Delete;

using System;
using FluentValidation.TestHelper;
using MazadZone.Features.DisputeTypes.Commands.Delete;
using Xunit;

public class DeleteDisputeTypeCommandValidatorTests
{
    private readonly DeleteDisputeTypeCommandValidator _validator;

    public DeleteDisputeTypeCommandValidatorTests()
    {
        _validator = new DeleteDisputeTypeCommandValidator();
    }

    [Fact]
    public void Validate_WhenDisputeTypeIdIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = new DeleteDisputeTypeCommand(DisputeTypeId.New());

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
        var command = new DeleteDisputeTypeCommand(DisputeTypeId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisputeTypeId);
    }
}