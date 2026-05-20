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
    => new VerifyBidderCommand(BidderId.New());

    public static Bidder CreateUnverifiedBidder(BidderId id)
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
            BidderId.New(),                                  // Id
            "Omar Abumuhfouz",                               // FullName
            "omar.abumuhfouz@mazadzone.com",                 // Email
            "0791234567",                                    // PhoneNumber
            new AddressDto("Jordan", "Amman", "Queen Rania St", "11118"), // Address
            0,                                               // TotalBidsPlaced
            0.0m,                                            // ReliabilityScore (decimal)
            "9876543210",                                    // NationalId (Added to match record signature)
            DateTime.UtcNow                                  // MemberSince
        );
    }
}