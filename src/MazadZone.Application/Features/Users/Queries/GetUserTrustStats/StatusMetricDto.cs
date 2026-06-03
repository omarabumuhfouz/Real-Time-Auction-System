namespace MazadZone.Application.Features.Users.Queries.GetUserTrustStats;

public record StatusMetricDto(int Count, decimal Percentage);

public record TrustScoreDto(decimal Score, decimal PercentageChange);

public record AccountStatusOverviewDto(
    StatusMetricDto Active,
    StatusMetricDto Suspended,
    StatusMetricDto Banned
);

public record UserTrustStatsDto(
    int TotalRegisteredUsers,
    int TotalSellersEnabled,
    AccountStatusOverviewDto StatusOverview,
    TrustScoreDto TrustScore
);