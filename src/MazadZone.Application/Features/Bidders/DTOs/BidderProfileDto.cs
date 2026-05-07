using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Bidders.DTOs;

public record BidderProfileDto(
    BidderId Id,
    string FullName,
    string Email,
    string PhoneNumber,
    AddressDto Address,
    int TotalBidsPlaced,      // Stats increase engagement
    decimal ReliabilityScore, // Based on completed payments/wins
    DateTime MemberSince
);