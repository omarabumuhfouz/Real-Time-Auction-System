using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;

public sealed record GetPublicSellerProfileQuery(SellerId SellerId) : IQuery<PublicSellerProfileResponse>;