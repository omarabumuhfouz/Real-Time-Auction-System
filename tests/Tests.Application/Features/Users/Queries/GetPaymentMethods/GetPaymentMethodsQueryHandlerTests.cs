namespace Tests.Application.Features.Users.Queries.GetPaymentMethods;

using MazadZone.Application.Features.Users.Queries.GetPaymentMethods;
using MazadZone.Application.Features.Users.Queries.GetProfileSettings;
using MazadZone.Domain.Users.Enums;
using MazadZone.Domain.Users.Errors;

public class GetPaymentMethodsQueryHandlerTests : UserBaseTest<GetPaymentMethodsQueryHandler>
{
    [Fact]
    public async Task Handle_UserFound_ReturnsPaymentMethods()
    {
        // Arrange
        var userId = UserId.New();
        var query = new GetPaymentMethodsQuery(userId);

        var mockProfile = new ProfileSettingsResponse(
            userId.Value,
            "Jane Doe",
            "jane@example.com",
            "+123456789",
            "12345678901234",
            "City",
            "Street",
            "Building",
            "Landmark",
            "Verified",
            null,
            null);

        var mockPaymentMethods = new List<PaymentMethodResponse>
        {
            new(Guid.NewGuid(), "1234", 12, 2030, "Jane Doe", CardBrand.Visa, true, DateTime.UtcNow)
        };

        _userQueries.GetProfileSettings(userId, Arg.Any<CancellationToken>())
            .Returns(mockProfile);

        _userQueries.GetPaymentMethodsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(mockPaymentMethods);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Last4Digits.ShouldBe("1234");
        result.Value[0].IsDefault.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var userId = UserId.New();
        var query = new GetPaymentMethodsQuery(userId);

        _userQueries.GetProfileSettings(userId, Arg.Any<CancellationToken>())
            .Returns((ProfileSettingsResponse?)null);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);
    }
}
