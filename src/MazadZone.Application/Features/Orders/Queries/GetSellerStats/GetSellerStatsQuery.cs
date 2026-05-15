using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Orders.Queries.GetSellerStats;

public record GetSellerStatsQuery(SellerId SellerId) : IQuery<SellerOrderStatsDto>;
