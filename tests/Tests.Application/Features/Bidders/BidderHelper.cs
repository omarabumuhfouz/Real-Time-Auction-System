using MazadZone.Application.Features.Bidders.Commands.Register;
using MazadZone.Application.Features.Bidders.Commands.Verify;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Bidders;

public static class BidderHelper
{
    // --- Core Factory Helper ---
    public static RegisterBidderCommand CreateValidRegisterCommand()
    {
        // Supplying valid dummy data that will pass your custom extension guards
        return new RegisterBidderCommand(
            "omar.abumuhfouz@mazadzone.com",                 // Email
            "StrongP@ssw0rd2026!",                           // Password
            "0791234567",                                    // PhoneNumber
            "9876543210",                                    // NationalId (Added to match record signature)
            "Omar",                                          // FirstName
            "Ahmad",                                         // SecondName
            "Ali",                                           // ThirdName
            "Abumuhfouz",                                    // LastName
            new AddressDto("Jordan", "Amman", "Queen Rania St", "11118") // Address
        );
    }

    public static VerifyBidderCommand CreateVerifyBidderCommand()
    => new VerifyBidderCommand(UserId.New());

    public static Bidder CreateUnverifiedBidder(UserId id)
    {
        // Adjust this instantiation based on your Bidder domain factory methods
        // Since Bidder ID typically matches User ID, we load it directly
        var address = Address.Create("Jordan", "Amman", "Street", "11118").Value;

        // Assuming you have a CompleteProfile or similar factory method from previous handlers
        return Bidder.CompleteProfile(UserId.From(id.Value), "9991012345", address).Value;
    }

    public static BidderProfileDto CreateValidBidderProfileDto()
    {
        return new BidderProfileDto(
            Id: UserId.New(), 
            FullName: "Omar Abumuhfouz",
            Email: "omar.abumuhfouz@mazadzone.com",
            PhoneNumber: "0791234567",
            Status: "Active",
            IsVerified: true,
            MemberSince: DateTime.UtcNow.AddMonths(-12),
            LastLogin: DateTime.UtcNow.AddHours(-2),
            Address: new AddressDto("Jordan", "Amman", "Queen Rania St", "11118"),
            TotalBidsPlaced: 42,
            AuctionParticipatedCount: 10,
            AuctionsWonCount: 3,
            CompletedPurchasesCount: 3
        );
    }
}