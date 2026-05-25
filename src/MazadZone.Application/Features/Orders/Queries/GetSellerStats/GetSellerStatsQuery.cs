using MazadZone.Application.Features.Orders.Queries.DTOs;

namespace MazadZone.Application.Features.Orders.Queries.GetSellerStats;

public record GetSellerStatsQuery(UserId SellerId) : IQuery<SellerOrderStatsDto>;
