using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Queries.GetPrivateDetails;

public sealed record GetPrivateSellerDetailsQuery(SellerId SellerId) : IQuery<PrivateSellerDetailsResponse>;