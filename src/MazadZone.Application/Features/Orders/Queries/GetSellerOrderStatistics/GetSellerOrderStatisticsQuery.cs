namespace MazadZone.Application.Features.Orders.Queries.GetSellerOrderStatistics;

public record GetSellerOrderStatisticsQuery(UserId SellerId) : IQuery<OrderStatisticsDto>;