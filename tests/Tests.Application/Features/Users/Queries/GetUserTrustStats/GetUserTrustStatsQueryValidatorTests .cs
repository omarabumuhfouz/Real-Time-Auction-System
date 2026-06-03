namespace MazadZone.Application.Features.Users.Queries.GetUserTrustStats;

public class GetUserTrustStatsQueryValidatorTests
{
    private readonly GetUserTrustStatsQueryValidator _validator;

    public GetUserTrustStatsQueryValidatorTests()
    {
        _validator = new GetUserTrustStatsQueryValidator();
    }

    [Fact]
    public void Validate_WhenDatesAreValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var query = new GetUserTrustStatsQuery(
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateIsAfterEndDate_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserTrustStatsQuery(
            DateTime.UtcNow,             // Start is today
            DateTime.UtcNow.AddDays(-1)  // End is yesterday
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
              .WithErrorMessage("Start date must be before the end date.");
    }

    [Fact]
    public void Validate_WhenEndDateIsInTheFuture_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetUserTrustStatsQuery(
            DateTime.UtcNow.AddDays(-7),
            DateTime.UtcNow.AddDays(2)   // End is strictly in the future (beyond the 1-day buffer)
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
              .WithErrorMessage("End date cannot be in the future.");
    }
}