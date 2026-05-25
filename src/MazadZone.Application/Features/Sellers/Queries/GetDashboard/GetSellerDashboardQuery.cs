namespace MazadZone.Application.Features.Sellers.Queries.GetDashboard;

public sealed record GetSellerDashboardQuery(UserId SellerId, SellerDashboardFilter? Filter = null) : IQuery<SellerDashboardResponse>;
