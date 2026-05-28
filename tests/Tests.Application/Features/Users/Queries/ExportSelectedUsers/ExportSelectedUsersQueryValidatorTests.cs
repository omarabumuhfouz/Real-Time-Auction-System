namespace Tests.Application.Features.Users.Queries.ExportSelectedUsers;

using System;
using System.Collections.Generic;
using FluentValidation.TestHelper;
using MazadZone.Application.Features.Users.Queries.ExportSelectedUsers;
using Xunit;

public class ExportSelectedUsersQueryValidatorTests
{
    private readonly ExportSelectedUsersQueryValidator _validator;

    public ExportSelectedUsersQueryValidatorTests()
    {
        _validator = new ExportSelectedUsersQueryValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = new ExportSelectedUsersQuery(new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenSelectedUserIdsListIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new ExportSelectedUsersQuery(new List<Guid>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SelectedUserIds);
    }

    [Fact]
    public void Validate_WhenSelectedUserIdsListIsNull_ShouldHaveValidationError()
    {
        // Arrange
        // Passing null to ensure FluentValidation's NotEmpty handles it gracefully
        var command = new ExportSelectedUsersQuery(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SelectedUserIds);
    }
}