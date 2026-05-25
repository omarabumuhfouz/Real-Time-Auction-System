using Dapper;
using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Application.Services;
using MazadZone.Domain.Users.ValueObjects;
using Polly;

namespace MazadZone.Infrastructure.Repositories.Queries;

public class BidderQueries : ResilientRepository, IBidderQueries
{
    public BidderQueries(ISqlConnectionFactory sqlFactory, IAsyncPolicy resiliencePolicy) : base(sqlFactory, resiliencePolicy)
    {
    }

    public async Task<BidderProfileDto?> GetBidderProfile(UserId bidderId, CancellationToken cancellationToken = default)
    {
        var query = @"
        SELECT 
          b.Id,
          u.FirstName + ' ' + u.LastName AS FullName,
          u.Email,
          u.PhoneNumber,

          CASE u.Status
          WHEN 1 THEN 'Active' 
          WHEN 2 THEN 'Suspended' 
          WHEN 3 THEN 'Banned' 
          ELSE 'UnKnown'
          END AS Status,

          b.IsVerified,
          u.CreatedOnUtc AS MemberSince,
          u.LastLogin,
          a.City,
          a.Street,
          a.Building,
          a.Landmark,
          COALESCE(b.TotalPidsPlaced, 0) AS TotalBidsPlaced,
          COALESCE(b.AuctionParticipatedCount, 0) AS AuctionParticipatedCount,
          COALESCE(b.AuctionsWonCount, 0) AS AuctionsWonCount,
          COALESCE(b.CompletedPurchasesCount, 0) AS CompletedPurchasesCount
        FROM Bidders b
        JOIN Users u
        WHERE b.Id = @bidderId
        ";

        return await ExecuteResilientAsync(connection =>
            connection.QueryFirstOrDefaultAsync<BidderProfileDto>(query, new
            {
                bidderId = bidderId.Value
            })
        );
    }
}