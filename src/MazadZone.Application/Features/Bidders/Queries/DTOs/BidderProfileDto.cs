
namespace MazadZone.Application.Features.Bidders.DTOs;

public record BidderProfileDto(
    Guid Id, // Note: Changed from UserId to Guid for seamless Dapper mapping, see note below
    string FullName,
    string Email,
    string PhoneNumber,
    string Status,
    bool IsVerified,
    DateTime MemberSince,
    DateTime LastLogin, // Made nullable in case LastLogin can be null in DB
    string City,
    string Street,
    string Building,
    string Landmark, // Made nullable if landmark is optional
    int TotalBidsPlaced,      
    int AuctionParticipatedCount,
    int AuctionsWonCount,
    int CompletedPurchasesCount
);