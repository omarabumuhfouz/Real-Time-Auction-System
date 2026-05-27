using System.Globalization;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Dashboard.Queries.GetAuctionActivityTrends;

public class GetAuctionActivityTrendsQueryHandler : IQueryHandler<GetAuctionActivityTrendsQuery, AuctionActivityTrendsDto>
{
    private readonly IDashboardQueries _dashboardQueries;

    public GetAuctionActivityTrendsQueryHandler(IDashboardQueries dashboardQueries)
    {
        _dashboardQueries = dashboardQueries;
    }

    public async Task<Result<AuctionActivityTrendsDto>> Handle(GetAuctionActivityTrendsQuery request, CancellationToken ct)
    {
        var duration = request.EndDate - request.StartDate;
        var prevStartDate = request.StartDate - duration;
        var prevEndDate = request.StartDate;

        var parsedPeriod = Enum.Parse<ChartGroupingPeriod>(request.Period, ignoreCase: true);

        var result = await _dashboardQueries.GetActivityTrendsAsync(
            request.StartDate, request.EndDate, 
            prevStartDate, prevEndDate, ct);

        // 1. Calculate Growth Percentages
        var auctionsGrowth = CalculateGrowth(result.Totals.CurrTotalAuctions, result.Totals.PrevTotalAuctions);
        var bidsGrowth = CalculateGrowth(result.Totals.CurrTotalBids, result.Totals.PrevTotalBids);

        // 2. Bucket the Data into the Requested Periods
        var chartData = BuildChartData(request.StartDate.Date, request.EndDate.Date, parsedPeriod, result.DailyAuctions, result.DailyBids);

        // 3. Find maximums for the frontend Y-Axes scaling
        var maxAuctions = chartData.Any() ? chartData.Max(x => x.NewAuctions) : 0;
        var maxBids = chartData.Any() ? chartData.Max(x => x.BidsPlaced) : 0;

        return new AuctionActivityTrendsDto(
            TotalNewAuctions: result.Totals.CurrTotalAuctions,
            AuctionsGrowthPercentage: auctionsGrowth,
            TotalBidsPlaced: result.Totals.CurrTotalBids,
            BidsGrowthPercentage: bidsGrowth,
            MaxAuctionsPoint: maxAuctions,
            MaxBidsPoint: maxBids,
            ChartData: chartData
        );
    }

    private static decimal CalculateGrowth(int current, int previous)
    {
        if (previous == 0) return current > 0 ? 100.0m : 0.0m;
        return Math.Round(((decimal)(current - previous) / previous) * 100, 1);
    }

    private static List<AuctionActivityDataPointDto> BuildChartData(
        DateTime start, DateTime end, ChartGroupingPeriod period, 
        IReadOnlyList<RawDailyActivity> rawAuctions, IReadOnlyList<RawDailyActivity> rawBids)
    {
        var points = new List<AuctionActivityDataPointDto>();
        var current = start;

        while (current < end)
        {
            var next = GetNextBoundary(current, period);
            if (next > end) next = end;

            // Aggregate data for this specific bucket
            var bucketAuctions = rawAuctions.Where(d => d.DatePoint >= current && d.DatePoint < next).Sum(d => d.Count);
            var bucketBids = rawBids.Where(d => d.DatePoint >= current && d.DatePoint < next).Sum(d => d.Count);

            points.Add(new AuctionActivityDataPointDto(
                Label: FormatLabel(current, next, period),
                NewAuctions: bucketAuctions,
                BidsPlaced: bucketBids
            ));

            current = next;
        }

        return points;
    }

    private static DateTime GetNextBoundary(DateTime current, ChartGroupingPeriod period) => period switch
    {
        ChartGroupingPeriod.Day => current.AddDays(1),
        ChartGroupingPeriod.Week => current.AddDays(7),
        ChartGroupingPeriod.Month => current.AddMonths(1),
        ChartGroupingPeriod.Quarter => current.AddMonths(3),
        ChartGroupingPeriod.Year => current.AddYears(1),
        _ => current.AddDays(1)
    };

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