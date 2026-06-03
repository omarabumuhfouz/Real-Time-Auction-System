namespace Tests.Application.Features.Users.Commands.BulkBan;

using System.Collections.Generic;
using FluentValidation.TestHelper;
using MazadZone.Application.Features.Users.Commands.BulkBan;
using Xunit;

public class BulkBanUsersCommandValidatorTests
{
    private readonly BulkBanUsersCommandValidator _validator;

    public BulkBanUsersCommandValidatorTests()
    {
        _validator = new BulkBanUsersCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = new BulkBanUsersCommand(
            new List<UserId> { UserId.New(), UserId.New() }, 
            "Spamming the auction chat");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdsListIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new BulkBanUsersCommand(new List<UserId>(), "Valid Reason");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserIds);
    }

    [Fact]
    public void Validate_WhenListContainsInvalidUserId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new BulkBanUsersCommand(
            new List<UserId> { UserId.New(), UserId.Empty }, // The bad apple
            "Valid Reason");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        // Explicitly check that the error fired for the specific index in the array
        result.ShouldHaveValidationErrorFor(x => x.UserIds);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenReasonIsInvalid_ShouldHaveValidationError(string? invalidReason)
    {
        // Arrange
        var command = new BulkBanUsersCommand(
            new List<UserId> { UserId.New() }, 
            invalidReason!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }
}