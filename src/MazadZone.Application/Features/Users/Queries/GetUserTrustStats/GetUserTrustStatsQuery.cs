namespace MazadZone.Application.Features.Users.Queries.GetUserTrustStats;

public record GetUserTrustStatsQuery(DateTime StartDate, DateTime EndDate) : IQuery<UserTrustStatsDto>;