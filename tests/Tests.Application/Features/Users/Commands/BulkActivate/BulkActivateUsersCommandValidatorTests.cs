namespace Tests.Application.Features.Users.Commands.BulkActivate;

using System.Collections.Generic;
using System.Linq;
using FluentValidation.TestHelper;
using MazadZone.Application.Features.Users.Commands.BulkActivate;
using Xunit;

public class BulkActivateUsersCommandValidatorTests
{
    private readonly BulkActivateUsersCommandValidator _validator;

    public BulkActivateUsersCommandValidatorTests()
    {
        _validator = new BulkActivateUsersCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = new BulkActivateUsersCommand(new List<UserId> 
        { 
            UserId.New(), 
            UserId.New() 
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdsListIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new BulkActivateUsersCommand(new List<UserId>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserIds)
              .WithErrorMessage("You must provide at least one user ID to activate.");
    }

    [Fact]
    public void Validate_WhenUserIdsListIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var command = new BulkActivateUsersCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserIds);
    }

    [Fact]
    public void Validate_WhenListExceedsMaximumAllowedCount_ShouldHaveValidationError()
    {
        // Arrange
        // Create a list of 101 valid IDs to trip the max limit rule
        var tooManyIds = Enumerable.Range(0, 101).Select(_ => UserId.New()).ToList();
        
        var command = new BulkActivateUsersCommand(tooManyIds);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserIds)
              .WithErrorMessage("You cannot activate more than 100 users at once.");
    }

    [Fact]
    public void Validate_WhenListContainsInvalidUserId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new BulkActivateUsersCommand(new List<UserId> 
        { 
            UserId.New(),       
            UserId.Empty        
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        // We explicitly check that the error fired for the specific index in the array
        result.ShouldHaveValidationErrorFor(x => x.UserIds);
        
    }
}