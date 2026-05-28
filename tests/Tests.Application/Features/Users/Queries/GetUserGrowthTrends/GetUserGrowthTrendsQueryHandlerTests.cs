using MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;

namespace Tests.Application.Features.Users.Queries.GetUserGrowthTrends;

public class GetUserGrowthTrendsQueryHandlerTests : UserBaseTest<GetUserGrowthTrendsQueryHandler>
{
    [Fact]
    public async Task Handle_CalculatesGrowthAndBucketsData_CorrectlyForWeeks()
    {
        // Arrange
        // 14 day period exactly (2 weeks)
        var startDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc); 
        var command = new GetUserGrowthTrendsQuery(startDate, endDate, "Week");

        var totals = new RawUserGrowthTotals(
            CurrTotalUsers: 150, PrevTotalUsers: 100, // 50% user growth expected
            CurrTotalSellers: 20, PrevTotalSellers: 25   // -20% seller growth expected
        );

        var dailyData = new List<RawDailyUserGrowth>
        {
            new(startDate.AddDays(1), 10, 2),  // Week 1 Bucket
            new(startDate.AddDays(5), 20, 3),  // Week 1 Bucket
            new(startDate.AddDays(10), 15, 5)  // Week 2 Bucket
        };

        var mockResult = new UserGrowthDataResult(totals, dailyData);

        _userQueries.GetUserGrowthTrendsAsync(
            startDate, endDate, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(mockResult);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        var dto = result.Value;
        dto.TotalNewUsers.ShouldBe(150);
        dto.UsersGrowthPercentage.ShouldBe(50.0m); 
        dto.TotalNewSellers.ShouldBe(20);
        dto.SellersGrowthPercentage.ShouldBe(-20.0m);
        
        // Max points across the buckets (Week 1 has 30 users, Week 2 has 15)
        dto.MaxUsersPoint.ShouldBe(30); 
        dto.MaxSellersPoint.ShouldBe(5); // Week 2 has max 5 sellers

        // Verify Chart Bucketing (Should be 2 full weeks)
        dto.ChartData.Count.ShouldBe(2);

        // Verify Week 1 Bucket (First)
        dto.ChartData.First().Label.ShouldBe("May 1 - May 7");
        dto.ChartData.First().NewUsers.ShouldBe(30);
        dto.ChartData.First().NewSellers.ShouldBe(5);

        // 💡 FIX: Used .Last() to check the second item in the list
        // Verify Week 2 Bucket (Last)
        dto.ChartData.Last().Label.ShouldBe("May 8 - May 14");
        dto.ChartData.Last().NewUsers.ShouldBe(15);
        dto.ChartData.Last().NewSellers.ShouldBe(5);
    }

    [Fact]
    public async Task Handle_ZeroPreviousUsers_HandlesDivideByZeroSafely()
    {
        // Arrange
        var startDate = new DateTime(2026, 5, 1);
        var command = new GetUserGrowthTrendsQuery(startDate, startDate.AddDays(1), "Day");

        var totals = new RawUserGrowthTotals(CurrTotalUsers: 10, PrevTotalUsers: 0, CurrTotalSellers: 0, PrevTotalSellers: 0);
        
        _userQueries.GetUserGrowthTrendsAsync(default, default, default, default, default)
            .ReturnsForAnyArgs(new UserGrowthDataResult(totals, new List<RawDailyUserGrowth>()));

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        // Current > 0 and Prev == 0 should yield exactly 100%
        result.Value.UsersGrowthPercentage.ShouldBe(100.0m);
        // Current == 0 and Prev == 0 should yield exactly 0%
        result.Value.SellersGrowthPercentage.ShouldBe(0.0m); 
    }

    [Fact]
    public async Task Handle_ValidatesMonthAndQuarterLabels()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 7, 1); 
        var command = new GetUserGrowthTrendsQuery(startDate, endDate, "Quarter"); // Testing "Quarter" period

        var totals = new RawUserGrowthTotals(0, 0, 0, 0);
        var dailyData = new List<RawDailyUserGrowth> { new(new DateTime(2026, 2, 1), 10, 1) }; // Falls in Q1

        _userQueries.GetUserGrowthTrendsAsync(default, default, default, default, default)
            .ReturnsForAnyArgs(new UserGrowthDataResult(totals, dailyData));

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ChartData.Count.ShouldBe(2); // Q1 (Jan-Mar) and Q2 (Apr-Jun)
        
        // Check Q1 (First)
        result.Value.ChartData.First().Label.ShouldBe("Q1 2026");
        result.Value.ChartData.First().NewUsers.ShouldBe(10); // From the Feb 1 data point

        // 💡 FIX: Used .Last() to check the second item in the list
        // Check Q2 (Last)
        result.Value.ChartData.Last().Label.ShouldBe("Q2 2026");
        result.Value.ChartData.Last().NewUsers.ShouldBe(0);
    }
}