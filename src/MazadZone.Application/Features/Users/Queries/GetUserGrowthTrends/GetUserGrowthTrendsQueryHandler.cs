using System.Globalization;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;

public class GetUserGrowthTrendsQueryHandler : IQueryHandler<GetUserGrowthTrendsQuery, UserGrowthTrendsDto>
{
    private readonly IUserQueries _userQueries;

    public GetUserGrowthTrendsQueryHandler(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    public async Task<Result<UserGrowthTrendsDto>> Handle(GetUserGrowthTrendsQuery request, CancellationToken ct)
    {
        var duration = request.EndDate - request.StartDate;
        var prevStartDate = request.StartDate - duration;
        var prevEndDate = request.StartDate;

        var result = await _userQueries.GetUserGrowthTrendsAsync(
            request.StartDate, request.EndDate, 
            prevStartDate, prevEndDate, ct);

        // 1. Calculate Growth Percentages
        var usersGrowth = CalculateGrowth(result.Totals.CurrTotalUsers, result.Totals.PrevTotalUsers);
        var sellersGrowth = CalculateGrowth(result.Totals.CurrTotalSellers, result.Totals.PrevTotalSellers);

        // Safely parse the validated string into your Enum (ignoreCase: true handles "week", "WEEK", etc.)
        var parsedPeriod = Enum.Parse<ChartGroupingPeriod>(request.Period, ignoreCase: true);

        // 2. Bucket the Daily Data into the Requested Periods (Weeks, Months, etc.)
        var chartData = BuildChartData(request.StartDate.Date, request.EndDate.Date, parsedPeriod, result.DailyData);

        // 3. Find maximums for the frontend Y-Axis scaling
        var maxUsers = chartData.Any() ? chartData.Max(x => x.NewUsers) : 0;
        var maxSellers = chartData.Any() ? chartData.Max(x => x.NewSellers) : 0;

        return new UserGrowthTrendsDto(
            TotalNewUsers: result.Totals.CurrTotalUsers,
            UsersGrowthPercentage: usersGrowth,
            TotalNewSellers: result.Totals.CurrTotalSellers,
            SellersGrowthPercentage: sellersGrowth,
            MaxUsersPoint: maxUsers,
            MaxSellersPoint: maxSellers,
            ChartData: chartData
        );
    }

    private static decimal CalculateGrowth(int current, int previous)
    {
        if (previous == 0) return current > 0 ? 100.0m : 0.0m;
        return Math.Round(((decimal)(current - previous) / previous) * 100, 1);
    }

    private static List<UserGrowthDataPointDto> BuildChartData(DateTime start, DateTime end, ChartGroupingPeriod period, IReadOnlyList<RawDailyUserGrowth> rawData)
    {
        var points = new List<UserGrowthDataPointDto>();
        var current = start;

        while (current < end)
        {
            var next = GetNextBoundary(current, period);
            if (next > end) next = end;

            // Aggregate data for this specific bucket
            var bucketUsers = rawData.Where(d => d.DatePoint >= current && d.DatePoint < next).Sum(d => d.NewUsers);
            var bucketSellers = rawData.Where(d => d.DatePoint >= current && d.DatePoint < next).Sum(d => d.NewSellers);

            points.Add(new UserGrowthDataPointDto(
                Label: FormatLabel(current, next, period),
                NewUsers: bucketUsers,
                NewSellers: bucketSellers
            ));

            current = next;
        }

        return points;
    }

    // Logic to jump forward by 1 Week, 1 Month, etc.
    private static DateTime GetNextBoundary(DateTime current, ChartGroupingPeriod period) => period switch
    {
        ChartGroupingPeriod.Day => current.AddDays(1),
        ChartGroupingPeriod.Week => current.AddDays(7),
        ChartGroupingPeriod.Month => current.AddMonths(1),
        ChartGroupingPeriod.Quarter => current.AddMonths(3), 
        ChartGroupingPeriod.Year => current.AddYears(1),
        _ => current.AddDays(1)
    };

    // Formats the strings perfectly for the UI X-Axis (e.g. "May 6-12", "Apr 2026")
    private static string FormatLabel(DateTime start, DateTime end, ChartGroupingPeriod period) => period switch
    {
        ChartGroupingPeriod.Day => start.ToString("MMM d", CultureInfo.InvariantCulture),
        ChartGroupingPeriod.Week => $"{start.ToString("MMM d", CultureInfo.InvariantCulture)} - {end.AddDays(-1).ToString("MMM d", CultureInfo.InvariantCulture)}",
        ChartGroupingPeriod.Month => start.ToString("MMM yyyy", CultureInfo.InvariantCulture),
        ChartGroupingPeriod.Quarter => $"Q{((start.Month - 1) / 3) + 1} {start.Year}",
        ChartGroupingPeriod.Year => start.ToString("yyyy", CultureInfo.InvariantCulture),
        _ => start.ToString("MMM d")
    };
}