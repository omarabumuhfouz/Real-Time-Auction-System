using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Sellers.Queries.GetPrivateDetails;

public sealed record GetPrivateSellerDetailsQuery(SellerId SellerId) : IQuery<PrivateSellerDetailsResponse>;