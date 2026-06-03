using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;

namespace MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;
public record PublicSellerProfileResponse(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsVerified,
    DateTime MemberSince,
    DateTime LastLogin,
    decimal Rating,
    int ReviewsCount,
    int ListedAuctionsCount,
    int TotalBidsPlaced,      
    int AuctionParticipatedCount,
    int AuctionsWonCount,
    int CompletedPurchasesCount
    );