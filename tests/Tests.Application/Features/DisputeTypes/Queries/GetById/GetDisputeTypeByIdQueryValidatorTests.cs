namespace Tests.Application.Features.DisputeTypes.Queries.GetById;

using System;
using FluentValidation.TestHelper;
using MazadZone.Features.DisputeTypes.Queries.GetById;
using Xunit;

public class GetDisputeTypeByIdQueryValidatorTests
{
    private readonly GetDisputeTypeByIdQueryValidator _validator;

    public GetDisputeTypeByIdQueryValidatorTests()
    {
        _validator = new GetDisputeTypeByIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenDisputeTypeIdIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var query = new GetDisputeTypeByIdQuery(DisputeTypeId.New());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDisputeTypeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        // Passing Guid.Empty to ensure MustBeValidDisputeTypeId() correctly blocks it
        var query = new GetDisputeTypeByIdQuery(DisputeTypeId.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisputeTypeId);
    }
}