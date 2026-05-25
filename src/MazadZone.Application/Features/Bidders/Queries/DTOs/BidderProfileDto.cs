using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Bidders.DTOs;

public record BidderProfileDto(
    UserId Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Status,
    bool IsVerified,
    DateTime MemberSince,
    DateTime LastLogin,
    AddressDto Address,
    int TotalBidsPlaced,      
    int AuctionParticipatedCount,
    int AuctionsWonCount,
    int CompletedPurchasesCount
);