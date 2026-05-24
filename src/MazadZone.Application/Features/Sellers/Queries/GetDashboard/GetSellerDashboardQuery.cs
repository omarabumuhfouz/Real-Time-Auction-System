using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Queries.GetDashboard;

public sealed record GetSellerDashboardQuery(SellerId SellerId, SellerDashboardFilter? Filter = null) : IQuery<SellerDashboardResponse>;
