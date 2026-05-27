using MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Users.Queries.GetUserTrustStats;

public class GetUserTrustStatsQueryHandler : IQueryHandler<GetUserTrustStatsQuery, UserTrustStatsDto>
{
    private readonly IUserQueries _userQueries;

    public GetUserTrustStatsQueryHandler(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    public async Task<Result<UserTrustStatsDto>> Handle(GetUserTrustStatsQuery request, CancellationToken ct)
    {
        // Calculate the previous period dynamically (e.g., if requesting 7 days, previous is the 7 days before that)
        var duration = request.EndDate - request.StartDate;
        var prevStartDate = request.StartDate - duration;
        var prevEndDate = request.StartDate;

        var metrics = await _userQueries.GetUserTrustMetricsAsync(
            request.StartDate, request.EndDate, 
            prevStartDate, prevEndDate, 
            ct);

        // Safe percentage calculations to avoid DivideByZero exceptions
        var total = metrics.TotalUsers == 0 ? 1 : metrics.TotalUsers;

        var activePct = Math.Round(((decimal)metrics.ActiveAccounts / total) * 100, 1);
        var suspendedPct = Math.Round(((decimal)metrics.SuspendedAccounts / total) * 100, 1);
        var bannedPct = Math.Round(((decimal)metrics.BannedAccounts / total) * 100, 1);

        // Current Overall Trust Score Logic: Percentage of users who are NOT banned
        var currentTrustScore = Math.Round(((decimal)(metrics.TotalUsers - metrics.BannedAccounts) / total) * 100, 1);

        // Dynamic Period Change calculation
        var currPeriodScore = CalculateScore(metrics.CurrentPeriodGoodAccounts, metrics.CurrentPeriodTotalAccounts);
        var prevPeriodScore = CalculateScore(metrics.PreviousPeriodGoodAccounts, metrics.PreviousPeriodTotalAccounts);
        
        var trustScoreChange = prevPeriodScore == 0 
            ? (currPeriodScore > 0 ? 100.0m : 0.0m) 
            : Math.Round(currPeriodScore - prevPeriodScore, 1);

        return new UserTrustStatsDto(
            TotalRegisteredUsers: metrics.TotalUsers,
            TotalSellersEnabled: metrics.TotalSellers,
            StatusOverview: new AccountStatusOverviewDto(
                Active: new StatusMetricDto(metrics.ActiveAccounts, activePct),
                Suspended: new StatusMetricDto(metrics.SuspendedAccounts, suspendedPct),
                Banned: new StatusMetricDto(metrics.BannedAccounts, bannedPct)
            ),
            TrustScore: new TrustScoreDto(currentTrustScore, trustScoreChange)
        );
    }

    private static decimal CalculateScore(int good, int total)
    {
        if (total == 0) return 0;
        return ((decimal)good / total) * 100;
    }
}