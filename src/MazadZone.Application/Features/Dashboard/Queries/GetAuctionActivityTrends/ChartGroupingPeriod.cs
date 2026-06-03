namespace MazadZone.Application.Features.Dashboard.Queries.GetAuctionActivityTrends;

public enum ChartGroupingPeriod
{
    Day,
    Week,
    Month,
    Quarter,
    Year
}

public record AuctionActivityDataPointDto(
    string Label, 
    int NewAuctions, 
    int BidsPlaced
);

public record AuctionActivityTrendsDto(
    int TotalNewAuctions,
    decimal AuctionsGrowthPercentage,
    int TotalBidsPlaced,
    decimal BidsGrowthPercentage,
    int MaxAuctionsPoint, // Helps frontend scale the Orange Bars
    int MaxBidsPoint,     // Helps frontend scale the Black Line
    List<AuctionActivityDataPointDto> ChartData
);