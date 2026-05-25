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
            u.Id , 
            u.FirstName + ' ' + u.LastName AS FullName,
            u.Email,
            u.PhoneNumber,
            s.IsVerified,
            u.CreatedOnUtc AS MemberSince,
            u.LastLogin,
            s.Rating, 
            s.ReviewsCount, 
            s.ListedAuctionsCount,
            b.TotalBidsPlaced,
            b.AuctionParticipatedCount,
            b.AuctionsWonCount,
            b.CompletedPurchasesCount
            f.Id as FeedbackId,
            f.Rating,
                COALESCE(b.TotalPidsPlaced, 0) AS TotalBidsPlaced,
                COALESCE(b.AuctionParticipatedCount, 0) AS AuctionParticipatedCount,
                COALESCE(b.AuctionsWonCount, 0) AS AuctionsWonCount,
                COALESCE(b.CompletedPurchasesCount, 0) AS CompletedPurchasesCount
            
            FROM Users 
            JOIN Sellers s b ON u.Id = s.Id
            JOIN Bidders b ON u.Id = b.Id
            WHERE u.Id = @SellerId;

            -- Get Feedbacks

            SELECT 
            f.Id,
            u.FirstName + ' ' + u.LastName AS AuthorName,
            f.Rating,
            f.Comment,
            f.Reply,
            f.CreatedAtUtc AS CreatedAt
            FROM Orders o
            JOIN Feedbacks f on o.Id = f.OrderId
            JOIN Users u on o.BidderId = u.Id 
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

        const string sql = """
            SELECT 
                u.Id , 
                u.FirstName
                CreatedOnUtc AS JoinedOn
            FROM Sellers
            JOIN Users u ON u.Id = Sellers.Id
            WHERE IsVerified = 0
            ORDER BY CreatedOnUtc ASC
            """;

        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<UnverifiedSellerSummaryResponse>(command);

        return result.AsList();
    }

    public async Task<SellerDashboardResponse?> GetSellerDashboardAsync(UserId sellerId, SellerDashboardFilter? filter, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Stats
        const string activeSql = "SELECT COUNT(1) FROM Auctions WHERE SellerId = @SellerId AND Status = @ActiveStatus";
        const string pendingSql = "SELECT COUNT(1) FROM Auctions WHERE SellerId = @SellerId AND Status = @PendingStatus";
        const string soldSql = "SELECT COUNT(1) FROM Auctions a WHERE a.SellerId = @SellerId AND a.Status = @EndedStatus AND EXISTS (SELECT 1 FROM Bids b WHERE b.AuctionId = a.Id)";
        const string unsoldSql = "SELECT COUNT(1) FROM Auctions a WHERE a.SellerId = @SellerId AND a.Status = @EndedStatus AND NOT EXISTS (SELECT 1 FROM Bids b WHERE b.AuctionId = a.Id)";

        var active = await connection.ExecuteScalarAsync<int>(new CommandDefinition(activeSql, new { SellerId = sellerId.Value, ActiveStatus = (int)AuctionStatus.Active }, cancellationToken: cancellationToken));
        var pending = await connection.ExecuteScalarAsync<int>(new CommandDefinition(pendingSql, new { SellerId = sellerId.Value, PendingStatus = (int)AuctionStatus.Pending }, cancellationToken: cancellationToken));
        var sold = await connection.ExecuteScalarAsync<int>(new CommandDefinition(soldSql, new { SellerId = sellerId.Value, EndedStatus = (int)AuctionStatus.Ended }, cancellationToken: cancellationToken));
        var unsold = await connection.ExecuteScalarAsync<int>(new CommandDefinition(unsoldSql, new { SellerId = sellerId.Value, EndedStatus = (int)AuctionStatus.Ended }, cancellationToken: cancellationToken));

        // Auctions list (paged)
        var page = filter?.Page > 0 ? filter.Page : 1;
        var pageSize = filter?.PageSize > 0 ? filter.PageSize : 20;
        var offset = (page - 1) * pageSize;

        var whereClause = "WHERE a.SellerId = @SellerId";
        if (!string.IsNullOrWhiteSpace(filter?.Status))
        {
            // Try parse status name to AuctionStatus enum value
            if (Enum.TryParse<AuctionStatus>(filter.Status, true, out var s))
            {
                whereClause += " AND a.Status = @Status";
            }
        }

        var listSql = $"SELECT a.Id AS AuctionId, it.Title, c.Name AS Category, a.Status, " +
                      "(SELECT COUNT(1) FROM Bids b WHERE b.AuctionId = a.Id) AS BidsCount, " +
                      "(SELECT TOP(1) b.Amount FROM Bids b WHERE b.AuctionId = a.Id ORDER BY b.PlacedAtUtc DESC) AS LastBidAmount, " +
                      "a.EndTime AS EndDateUtc, " +
                      "(SELECT TOP(1) img.ImageUrl FROM Images img WHERE img.ItemId = it.Id AND img.isMain = 1) AS ThumbnailUrl " +
                      "FROM Auctions a " +
                      "INNER JOIN Items it ON it.AuctionId = a.Id " +
                      "LEFT JOIN Categories c ON c.Id = it.CategoryId " +
                      whereClause +
                      " ORDER BY a.EndTime DESC " +
                      " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var parameters = new { SellerId = sellerId.Value, Status = filter?.Status != null ? (object)filter.Status : null, Offset = offset, PageSize = pageSize };

        var auctions = await connection.QueryAsync<SellerAuctionSummaryDto>(new CommandDefinition(listSql, parameters, cancellationToken: cancellationToken));

        return new SellerDashboardResponse
        {
            ActiveAuctions = active,
            Pending = pending,
            SoldItems = sold,
            Unsold = unsold,
            Auctions = auctions.AsList()
        };
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
