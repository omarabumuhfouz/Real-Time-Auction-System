using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;

public sealed record GetPublicSellerProfileQuery(SellerId SellerId) : IQuery<PublicSellerProfileResponse>;