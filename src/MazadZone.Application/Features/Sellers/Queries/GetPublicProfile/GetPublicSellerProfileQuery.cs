using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;

public sealed record GetPublicSellerProfileQuery(UserId SellerId) : IQuery<PublicSellerProfileResponse>;