using MazadZone.Application.Features.Dashboard.Queries.GetAuctionActivityTrends;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

public record PeriodStats(int TotalAuctions, int LiveAuctions, int EndingWithin24h, int CompletedOrders, int OpenDisputes);

public interface IDashboardQueries : IScopedService
{
    Task<(PeriodStats Current, PeriodStats Previous)> GetDashboardMetricsAsync(
        DateTime currStart, DateTime currEnd,
        DateTime prevStart, DateTime prevEnd,
        DateTime utcNow,
        CancellationToken ct);

    Task<AuctionActivityDataResult> GetActivityTrendsAsync(
           DateTime currStart, DateTime currEnd,
           DateTime prevStart, DateTime prevEnd,
           CancellationToken ct);


}