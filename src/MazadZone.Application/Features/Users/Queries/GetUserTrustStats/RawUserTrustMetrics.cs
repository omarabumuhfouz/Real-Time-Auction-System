namespace MazadZone.Application.Features.Users.Queries.GetUserTrustStats;

public record RawUserTrustMetrics(
    int TotalUsers,
    int TotalSellers,
    int ActiveAccounts,
    int SuspendedAccounts,
    int BannedAccounts,
    int CurrentPeriodGoodAccounts,
    int CurrentPeriodTotalAccounts,
    int PreviousPeriodGoodAccounts,
    int PreviousPeriodTotalAccounts
);