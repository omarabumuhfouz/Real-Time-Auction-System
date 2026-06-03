using MazadZone.Application.Features.Dashboard.DTOs;

namespace MazadZone.Application.Features.Dashboard.Queries.GetStats;

public record GetDashboardStatsQuery(DateTime StartDate, DateTime EndDate) : IQuery<DashboardStatsDto>;