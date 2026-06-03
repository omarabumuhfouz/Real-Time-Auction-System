namespace MazadZone.Application.Features.Dashboard.Queries.GetAuctionActivityTrends;
public record GetAuctionActivityTrendsQuery(
    DateTime StartDate, 
    DateTime EndDate, 
    string Period // Accepts string directly from the frontend
) : IQuery<AuctionActivityTrendsDto>;