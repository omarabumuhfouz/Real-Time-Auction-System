namespace MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;

public record GetUserGrowthTrendsQuery(
    DateTime StartDate, 
    DateTime EndDate, 
    string Period
) : IQuery<UserGrowthTrendsDto>;