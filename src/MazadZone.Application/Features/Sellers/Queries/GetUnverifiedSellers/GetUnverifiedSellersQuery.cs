namespace MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;

public sealed record GetUnverifiedSellersQuery() : IQuery<IReadOnlyList<UnverifiedSellerSummaryResponse>>;