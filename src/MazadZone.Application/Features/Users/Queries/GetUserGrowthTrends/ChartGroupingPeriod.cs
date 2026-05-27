namespace MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;

public enum ChartGroupingPeriod
{
    Day,
    Week,
    Month,
    Quarter, 
    Year
}

public record UserGrowthDataPointDto(
    string Label, 
    int NewUsers, 
    int NewSellers
);

public record UserGrowthTrendsDto(
    int TotalNewUsers,
    decimal UsersGrowthPercentage,
    int TotalNewSellers,
    decimal SellersGrowthPercentage,
    int MaxUsersPoint,     // Helps frontend scale the Y-axis
    int MaxSellersPoint,   // Helps frontend scale the Y-axis
    List<UserGrowthDataPointDto> ChartData
);