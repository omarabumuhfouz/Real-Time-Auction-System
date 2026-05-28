namespace Tests.Application.Features.Users.Queries.GetUsers;

using FluentValidation.TestHelper;
using MazadZone.Application.Features.Users.Queries.GetUsers;
using MazadZone.Application.Features.Users.DTOs;
using Xunit;

public class GetUsersQueryValidatorTests
{
    private readonly GetUsersQueryValidator _validator;

    public GetUsersQueryValidatorTests()
    {
        _validator = new GetUsersQueryValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var filter = new UserFilterParams 
        { 
            PageNumber = 1, 
            PageSize = 50, 
            SearchTerm = "John", 
            JoinedDate = "2023-10-01" 
        };
        var query = new GetUsersQuery(filter);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenPageNumberIsLessThanOne_ShouldHaveValidationError(int invalidPageNumber)
    {
        var filter = new UserFilterParams { PageNumber = invalidPageNumber };
        var query = new GetUsersQuery(filter);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.FilterParams.PageNumber)
              .WithErrorMessage("Page number must be at least 1.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(101)]
    public void Validate_WhenPageSizeIsOutOfBounds_ShouldHaveValidationError(int invalidPageSize)
    {
        var filter = new UserFilterParams { PageSize = invalidPageSize };
        var query = new GetUsersQuery(filter);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.FilterParams.PageSize);
    }

    [Fact]
    public void Validate_WhenSearchTermIsTooLong_ShouldHaveValidationError()
    {
        // Arrange - Create a string 101 characters long
        var tooLongSearchTerm = new string('A', 101);
        var filter = new UserFilterParams { SearchTerm = tooLongSearchTerm };
        var query = new GetUsersQuery(filter);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FilterParams.SearchTerm);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WhenJoinedDateIsEmpty_ShouldNotHaveValidationError(string? emptyDate)
    {
        // Null or empty string is totally valid according to the Must() rule
        var filter = new UserFilterParams { JoinedDate = emptyDate };
        var query = new GetUsersQuery(filter);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(x => x.FilterParams.JoinedDate);
    }

    [Fact]
    public void Validate_WhenJoinedDateIsInvalidFormat_ShouldHaveValidationError()
    {
        // Arrange
        var filter = new UserFilterParams { JoinedDate = "not-a-valid-date" };
        var query = new GetUsersQuery(filter);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FilterParams.JoinedDate);
    }
}