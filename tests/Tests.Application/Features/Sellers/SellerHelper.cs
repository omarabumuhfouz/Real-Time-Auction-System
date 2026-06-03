using MazadZone.Application.Features.Sellers.Commands.BecomeSeller;
using MazadZone.Application.Features.Sellers.Commands.Verify;
using MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;
using MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;
using MazadZone.Domain.Sellers;

namespace Tests.Application.Features.Sellers;

public static class SellerHelper
{
    /// <summary>
    /// Centralizes the creation of a valid dummy Seller for testing purposes.
    /// </summary>
    public static Seller CreateValidSeller()
    {
        return Seller.BecomeSeller(
            UserId.New()
        ).Value;
    }

    public static BecomeSellerCommand CreateBecomeSellerCommand()
    {
        return new BecomeSellerCommand(UserId.New());
    }

    public static VerifySellerCommand CreateVerifyCommand()
    {
        return new VerifySellerCommand(UserId.New());
    }

    public static GetPublicSellerProfileQuery CreateGetPublicSellerProfileQuery() =>
          new GetPublicSellerProfileQuery(UserId.New());

    /// <summary>
    /// Centralizes the creation of a PublicSellerProfileResponse with sensible baseline mock data.
    /// </summary>
    public static PublicSellerProfileResponse CreatePublicSellerProfileResponse()
    {
        return new PublicSellerProfileResponse(
            Id: Guid.NewGuid(),
            FullName: "John Doe",
            Email: "john.doe@example.com",
            PhoneNumber: "+1234567890",
            IsVerified: true,
            MemberSince: DateTime.UtcNow.AddMonths(-6),
            LastLogin: DateTime.UtcNow.AddDays(-1),
            Rating: 4.9m,
            ReviewsCount: 120,
            ListedAuctionsCount: 15,
            TotalBidsPlaced: 50,
            AuctionParticipatedCount: 10,
            AuctionsWonCount: 5,
            CompletedPurchasesCount: 5
        );
    }

    /// <summary>
    /// Centralizes the creation of a mock list containing baseline UnverifiedSellerSummaryResponse records.
    /// </summary>
    public static List<UnverifiedSellerSummaryResponse> CreateUnverifiedSellerSummaries() =>
    [
        new(
        Id: Guid.NewGuid(),
        FullName: "Ahmad Hassan",
        Email: "ahmad.hassan@example.com",
        PhoneNumber: "+962790000001",
        JoinedOn: DateTime.UtcNow.AddDays(-2)
    ),
    new(
        Id: Guid.NewGuid(),
        FullName: "Sara Ali",
        Email: "sara.ali@example.com",
        PhoneNumber: "+962790000002",
        JoinedOn: DateTime.UtcNow.AddHours(-5)
    )
    ];
}