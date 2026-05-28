namespace Tests.Application.Features.Users.Commands.BulkSuspend;

using System;
using System.Collections.Generic;
using FluentValidation.TestHelper;
using MazadZone.Application.Features.Users.Commands.BulkSuspend;
// using MazadZone.Domain.Users.ValueObjects; // Where UserId lives
using Xunit;

public class BulkSuspendUsersCommandValidatorTests
{
    private readonly BulkSuspendUsersCommandValidator _validator;

    public BulkSuspendUsersCommandValidatorTests()
    {
        _validator = new BulkSuspendUsersCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = new BulkSuspendUsersCommand(
            new List<UserId> { UserId.New(), UserId.New() }, 
            "Investigating account behavior",
            DateTime.UtcNow.AddDays(3));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdsListIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new BulkSuspendUsersCommand(
            new List<UserId>(), 
            "Valid Reason",
            DateTime.UtcNow.AddDays(3));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserIds);
    }

    [Fact]
    public void Validate_WhenListContainsInvalidUserId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new BulkSuspendUsersCommand(
            new List<UserId> { UserId.New(), UserId.Empty }, // The bad apple
            "Valid Reason",
            DateTime.UtcNow.AddDays(3));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserIds);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenReasonIsInvalid_ShouldHaveValidationError(string? invalidReason)
    {
        // Arrange
        var command = new BulkSuspendUsersCommand(
            new List<UserId> { UserId.New() }, 
            invalidReason!,
            DateTime.UtcNow.AddDays(3));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void Validate_WhenUntilDateIsInThePast_ShouldHaveValidationError()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var command = new BulkSuspendUsersCommand(
            new List<UserId> { UserId.New() }, 
            "Valid Reason",
            pastDate);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Until);
    }
}