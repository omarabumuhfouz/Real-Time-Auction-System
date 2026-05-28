namespace Tests.Application.Features.Users.Queries.GetProfileSettings;

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Features.Users.Queries.GetProfileSettings;
using MazadZone.Domain.Users.Errors;

public class GetProfileSettingsQueryHandlerTests : UserBaseTest<GetProfileSettingsQueryHandler>
{
    [Fact]
    public async Task Handle_UserFound_ReturnsProfileSettingsResponse()
    {
        // Arrange
        // Assuming UserId is a Guid based on the ProfileSettingsResponse. 
        // If using strongly-typed UserId, change to UserId.New()
        var command = new GetProfileSettingsQuery(UserId.New()); 

        var expectedProfile = new ProfileSettingsResponse(
            command.UserId.Value,
            "John Doe",
            "john@example.com",
            "+1234567890",
            "12345678901234",
            "Amman",
            "Rainbow Street",
            "Building 10",
            "Near First Circle"
        );

        _userQueries.GetProfileSettings(command.UserId, Arg.Any<CancellationToken>())
            .Returns(expectedProfile);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        
        // Verify mapping/data integrity
        result.Value.Id.ShouldBe(command.UserId.Value);
        result.Value.FullName.ShouldBe("John Doe");
        result.Value.Email.ShouldBe("john@example.com");
        result.Value.City.ShouldBe("Amman");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new GetProfileSettingsQuery(UserId.New());

        // Mock the Dapper/read repository to return null
        _userQueries.GetProfileSettings(command.UserId, Arg.Any<CancellationToken>())
            .Returns((ProfileSettingsResponse?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);
    }
}