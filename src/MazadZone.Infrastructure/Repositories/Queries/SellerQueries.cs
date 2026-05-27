using Dapper;
using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Sellers.Queries;
using MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;
using MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;
using MazadZone.Application.Features.Sellers.Queries.GetDashboard;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Users.ValueObjects;
using Polly;
using MazadZone.Application.Features.Orders.Queries.DTOs;

namespace MazadZone.Infrastructure.Repositories;

public sealed class SellerQueries : ResilientRepository, ISellerQueries
{
    public SellerQueries(ISqlConnectionFactory sqlFactory, IAsyncPolicy resiliencePolicy) : base(sqlFactory, resiliencePolicy)
    {
    }


    public async Task<PublicSellerProfileResponse?> GetPublicProfileAsync(UserId sellerId, CancellationToken cancellationToken)
    {

        const string sql = @"
            SELECT 
    u.Id, 
    u.FirstName + ' ' + u.LastName AS FullName,
    u.Email,
    u.PhoneNumber,
    s.IsVerified,
    u.CreatedOnUtc AS MemberSince,
    u.LastLogin,
    s.Rating, 
    s.ReviewsCount, 
    s.ListedAuctionsCount,


            COALESCE(b.TotalPidsPlaced, 0) AS TotalBidsPlaced, 
            COALESCE(b.AuctionParticipatedCount, 0) AS AuctionParticipatedCount,
            COALESCE(b.AuctionsWonCount, 0) AS AuctionsWonCount,
            COALESCE(b.CompletedPurchasesCount, 0) AS CompletedPurchasesCount
FROM Users u 
JOIN Sellers s ON u.Id = s.Id
LEFT JOIN Bidders b ON u.Id = b.Id 
WHERE u.Id = @SellerId;


SELECT 
    f.Id,
    u.FirstName + ' ' + u.LastName AS AuthorName,
    f.Rating,
    f.Comment,
    f.Reply,
    f.CreatedAtUtc AS CreatedAt
FROM Orders o
JOIN Feedbacks f ON o.Id = f.OrderId
JOIN Users u ON o.BidderId = u.Id 
JOIN Auctions a ON o.AuctionId = a.Id
WHERE a.SellerId = @SellerId
ORDER BY f.CreatedAtUtc DESC;
            ";
        return await ExecuteResilientAsync(async connection =>
        {
            using var multi = await connection.QueryMultipleAsync(sql, new { SellerId = sellerId.Value });

            var baseProfile = await multi.ReadSingleOrDefaultAsync<ProfileBaseResult>();
            if (baseProfile is null) return null;

            var feedbacks = (await multi.ReadAsync<FeedbackDto>()).ToList();

            return new PublicSellerProfileResponse(
                Id: baseProfile.Id,
                FullName: baseProfile.FullName,
                Email: baseProfile.Email,
                PhoneNumber: baseProfile.PhoneNumber,
                IsVerified: baseProfile.IsVerified,
                MemberSince: baseProfile.MemberSince,
                LastLogin: baseProfile.LastLogin,
                Rating: baseProfile.Rating,
                ReviewsCount: baseProfile.ReviewsCount,
                ListedAuctionsCount: baseProfile.ListedAuctionsCount,
                TotalBidsPlaced: baseProfile.TotalBidsPlaced,
                AuctionParticipatedCount: baseProfile.AuctionParticipatedCount,
                AuctionsWonCount: baseProfile.AuctionsWonCount,
                CompletedPurchasesCount: baseProfile.CompletedPurchasesCount,
                Feedbacks: feedbacks
            );
        });


    }

    public async Task<IReadOnlyList<UnverifiedSellerSummaryResponse>?> GetUnverifiedSellersAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
            SELECT
                u.Id,
                u.FirstName + ' ' + u.LastName AS FullName,
                u.Email,
                u.PhoneNumber,
                u.CreatedOnUtc AS JoinedOn
            FROM Sellers s
            JOIN Users u ON u.Id = s.Id
            WHERE s.IsVerified = 0
            ORDER BY JoinedOn
            ";

        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<UnverifiedSellerSummaryResponse>(command);

        return result.AsList();
    }


    private record ProfileBaseResult(
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
}
