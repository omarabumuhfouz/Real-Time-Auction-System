namespace Tests.Application.Features.Users.Queries.GetUserGrowthTrends;

using MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;

public class GetUserGrowthTrendsQueryValidatorTests
{
    private readonly GetUserGrowthTrendsQueryValidator _validator;

    public GetUserGrowthTrendsQueryValidatorTests()
    {
        _validator = new GetUserGrowthTrendsQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var command = new GetUserGrowthTrendsQuery(
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow,
            "Month" // Exact case match
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("day")]
    [InlineData("WEEK")]
    [InlineData("mOnTh")]
    [InlineData("Quarter")]
    [InlineData("YEAR")]
    public void Validate_WhenPeriodHasDifferentCasing_ShouldPassValidation(string validCasedPeriod)
    {
        // Arrange
        var command = new GetUserGrowthTrendsQuery(
            DateTime.UtcNow.AddDays(-7),
            DateTime.UtcNow,
            validCasedPeriod
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert - Ensures the caseSensitive: false configuration works
        result.ShouldNotHaveValidationErrorFor(x => x.Period);
    }

    [Fact]
    public void Validate_WhenStartDateIsAfterEndDate_ShouldHaveValidationError()
    {
        // Arrange
        var command = new GetUserGrowthTrendsQuery(
            DateTime.UtcNow, // Start is today
            DateTime.UtcNow.AddDays(-1), // End is yesterday
            "Day"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Validate_WhenEndDateIsInTheFuture_ShouldHaveValidationError()
    {
        // Arrange
        var command = new GetUserGrowthTrendsQuery(
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2), // Too far into the future
            "Day"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Validate_WhenPeriodIsInvalidString_ShouldHaveValidationError()
    {
        // Arrange
        var command = new GetUserGrowthTrendsQuery(
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow,
            "Decade" // Not an enum value
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Period);
    }
}