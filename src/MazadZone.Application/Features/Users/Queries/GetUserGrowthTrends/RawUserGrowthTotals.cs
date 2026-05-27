namespace MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;

public record RawUserGrowthTotals(
    int CurrTotalUsers, int CurrTotalSellers,
    int PrevTotalUsers, int PrevTotalSellers
);

public record RawDailyUserGrowth(
    DateTime DatePoint,
    int NewUsers,
    int NewSellers
);

public record UserGrowthDataResult(
    RawUserGrowthTotals Totals,
    IReadOnlyList<RawDailyUserGrowth> DailyData
);