using MazadZone.Application.Features.Users.Queries.GetUserTrustStats;

namespace Tests.Application.Features.Users.Queries.GetUserTrustStats;

public class GetUserTrustStatsQueryHandlerTests : UserBaseTest<GetUserTrustStatsQueryHandler>
{
    [Fact]
    public async Task Handle_WithValidData_CalculatesMetricsAndPercentagesCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 5, 8, 0, 0, 0, DateTimeKind.Utc); // 7 days
        var query = new GetUserTrustStatsQuery(startDate, endDate);

        // Scenario: 
        // 100 total users, 10 are banned -> Overall Trust Score: 90%
        // Current Period: 50 accounts created, 40 are good -> Period Score: 80%
        // Previous Period: 50 accounts created, 30 are good -> Period Score: 60%
        // Expected Change: 80 - 60 = +20%
        var rawMetrics = new RawUserTrustMetrics(
            TotalUsers: 100,
            TotalSellers: 25,
            ActiveAccounts: 80,
            SuspendedAccounts: 10,
            BannedAccounts: 10,
            CurrentPeriodGoodAccounts: 40,
            CurrentPeriodTotalAccounts: 50,
            PreviousPeriodGoodAccounts: 30,
            PreviousPeriodTotalAccounts: 50
        );

        _userQueries.GetUserTrustMetricsAsync(
            startDate, endDate, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(rawMetrics);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        var dto = result.Value;
        dto.TotalRegisteredUsers.ShouldBe(100);
        dto.TotalSellersEnabled.ShouldBe(25);

        // Verify Status Percentages
        dto.StatusOverview.Active.Count.ShouldBe(80);
        dto.StatusOverview.Active.Percentage.ShouldBe(80.0m);
        
        dto.StatusOverview.Suspended.Count.ShouldBe(10);
        dto.StatusOverview.Suspended.Percentage.ShouldBe(10.0m);
        
        dto.StatusOverview.Banned.Count.ShouldBe(10);
        dto.StatusOverview.Banned.Percentage.ShouldBe(10.0m);

        // Verify Trust Score Logic
        dto.TrustScore.Score.ShouldBe(90.0m); // (100 - 10) / 100
        dto.TrustScore.PercentageChange.ShouldBe(20.0m); // 80% curr - 60% prev
    }

    [Fact]
    public async Task Handle_ZeroTotalUsers_HandlesDivideByZeroSafely()
    {
        // Arrange
        var query = new GetUserTrustStatsQuery(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

        var rawMetrics = new RawUserTrustMetrics(0, 0, 0, 0, 0, 0, 0, 0, 0);

        _userQueries.GetUserTrustMetricsAsync(default, default, default, default, default)
            .ReturnsForAnyArgs(rawMetrics);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        var dto = result.Value;
        dto.TotalRegisteredUsers.ShouldBe(0);
        
        dto.StatusOverview.Active.Percentage.ShouldBe(0.0m);
        dto.StatusOverview.Suspended.Percentage.ShouldBe(0.0m);
        dto.StatusOverview.Banned.Percentage.ShouldBe(0.0m);
        
        dto.TrustScore.Score.ShouldBe(0.0m);
        dto.TrustScore.PercentageChange.ShouldBe(0.0m);
    }

    [Fact]
    public async Task Handle_ZeroPreviousPeriodUsers_CalculatesTrustScoreChangeCorrectly()
    {
        // Arrange
        var query = new GetUserTrustStatsQuery(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

        var rawMetrics = new RawUserTrustMetrics(
            TotalUsers: 10, TotalSellers: 2, ActiveAccounts: 10, SuspendedAccounts: 0, BannedAccounts: 0,
            CurrentPeriodGoodAccounts: 5, CurrentPeriodTotalAccounts: 5, // 100% current score
            PreviousPeriodGoodAccounts: 0, PreviousPeriodTotalAccounts: 0 // 0 previous accounts
        );

        _userQueries.GetUserTrustMetricsAsync(default, default, default, default, default)
            .ReturnsForAnyArgs(rawMetrics);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        // As defined in your logic: if prev is 0 and curr > 0, the change is exactly 100.0m
        result.Value.TrustScore.PercentageChange.ShouldBe(100.0m);
    }
}

