namespace Tests.Application.Features.Users.Queries.GetProfileSettings;

using FluentValidation.TestHelper;
using MazadZone.Application.Features.Users.Queries.GetProfileSettings;
using Xunit;

public class GetProfileSettingsQueryValidatorTests
{
    private readonly GetProfileSettingsQueryValidator _validator;

    public GetProfileSettingsQueryValidatorTests()
    {
        _validator = new GetProfileSettingsQueryValidator();
    }

    [Fact]
    public void Validate_WhenUserIdIsValid_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var query = new GetProfileSettingsQuery(UserId.New()); // Or UserId.New()

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        // We pass Guid.Empty (or UserId.Empty) to trigger the MustBeValidUserId() failure
        var query = new GetProfileSettingsQuery(UserId.Empty); 

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}