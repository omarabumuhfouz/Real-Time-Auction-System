using Dapper;
using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Sellers.Queries;
using MazadZone.Application.Features.Sellers.Queries.GetPrivateDetails;
using MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;
using MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;
using MazadZone.Application.Features.Sellers.Queries.GetDashboard;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;

namespace MazadZone.Infrastructure.Repositories;

public sealed class SellerQueries : ISellerQueries
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SellerQueries(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PublicSellerProfileResponse?> GetPublicProfileAsync(SellerId sellerId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                Id AS SellerId, 
                Rating, 
                ReviewsCount, 
                IsVerified, 
                CreatedOnUtc AS JoinedOnUtc
            FROM Sellers
            WHERE Id = @SellerId
            """;

        var command = new CommandDefinition(sql, new { SellerId = sellerId.Value }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<PublicSellerProfileResponse>(command);
    }

    public async Task<PrivateSellerDetailsResponse?> GetPrivateProfileAsync(SellerId sellerId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                Id AS SellerId, 
                BankAccountNumber, 
                TaxIdentificationNumber, 
                IsVerified, 
                Rating
            FROM Sellers
            WHERE Id = @SellerId
            """;

        var command = new CommandDefinition(sql, new { SellerId = sellerId.Value }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<PrivateSellerDetailsResponse>(command);
    }

    public async Task<IReadOnlyList<UnverifiedSellerSummaryResponse>?> GetUnverifiedSellersAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                Id AS SellerId, 
                BankAccountNumber, 
                TaxIdentificationNumber,
                CreatedOnUtc AS JoinedOnUtc
            FROM Sellers
            WHERE IsVerified = 0
            ORDER BY CreatedOnUtc ASC
            """;

        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<UnverifiedSellerSummaryResponse>(command);
        
        return result.AsList();
    }

    public async Task<SellerDashboardResponse?> GetSellerDashboardAsync(SellerId sellerId, SellerDashboardFilter? filter, CancellationToken cancellationToken)
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
}
