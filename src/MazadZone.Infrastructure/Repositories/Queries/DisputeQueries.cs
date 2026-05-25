namespace  MazadZone.Infrastructure.Repositories.Queries;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Disputes.Queries;
using MazadZone.Domain.Orders;
using Polly;

public class DisputeQueries : ResilientRepository, IDisputeQueries
{
    public DisputeQueries(ISqlConnectionFactory sqlFactory, IAsyncPolicy resiliencePolicy) 
        : base(sqlFactory, resiliencePolicy) {}

    public async Task<DisputeDetailsDto?> GetDetailsByIdAsync(DisputeId disputeId, CancellationToken ct)
    {
        // 1. We use ALIASES (AS) so every column has a unique, predictable name
        var mainSql = @"
        SELECT
            d.Id, 
            d.Status, 
            dt.Name AS DisputeType, 
            d.Title, 
            d.Description,
            
            -- Auction Info Aliases
            a.Id AS AuctionId, 
            i.Title AS AuctionTitle, 
            a.EndTime, 
            o.TotalAmount AS FinalPrice,
            (SELECT TOP 1 ImageUrl From ItemImages WHERE ItemId = i.Id AND IsMain = 1) AS MainImageUrl,
            
            -- Bidder Info Aliases
            bidder.Id AS BidderId, 
            bidder.FirstName + ' ' + bidder.LastName AS BidderName, 
            bidder.Email AS BidderEmail,

            -- Seller Info Aliases
            seller.Id AS SellerId, 
            seller.FirstName + ' ' + seller.LastName AS SellerName, 
            seller.Email AS SellerEmail

        FROM Disputes d 
        JOIN DisputeTypes dt ON d.DisputeTypeId = dt.Id
        JOIN Orders o ON d.OrderId = o.Id
        JOIN Auctions a ON o.AuctionId = a.Id
        JOIN Items i ON a.Id = i.AuctionId
        JOIN Users bidder ON o.BidderId = bidder.Id
        JOIN Users seller ON a.SellerId = seller.Id
        WHERE d.Id = @DisputeId;
    ";

        var imagesSql = @"
        SELECT ImageUrl AS Path, AltText 
        FROM DisputeImages
        WHERE DisputeId = @DisputeId;
    ";

        return await ExecuteResilientAsync(async connection =>
        {
            // STEP 1: Query the main row as a 'dynamic' object
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(mainSql, new { DisputeId = disputeId.Value });

            if (row is null) return null;

            // STEP 2: Query the attachments directly into your DTO
            var attachments = await connection.QueryAsync<DisputeAttachmentDto>(imagesSql, new { DisputeId = disputeId.Value });

            // STEP 3: Manually map the dynamic row directly into your strict records
            return new DisputeDetailsDto(
                Id: row.Id,
                Status: row.Status,
                DisputeType: row.DisputeType,
                Title: row.Title,
                Description: row.Description,

                AuctionDetails: new AuctionDisputeInfo
                {
                    Id = row.AuctionId,
                    Title = row.AuctionTitle,
                    FinalPrice = row.FinalPrice,
                    EndTime = row.EndTime,
                    MainImageUrl = row.MainImageUrl
                },

                Parties: new List<DisputeParties>
                {
                new DisputeParties
                {
                    Bidder = new DisputeUserDto
                    {
                        Id = row.BidderId,
                        Name = row.BidderName,
                        Email = row.BidderEmail
                    },
                    Seller = new DisputeUserDto
                    {
                        Id = row.SellerId,
                        Name = row.SellerName,
                        Email = row.SellerEmail
                    }
                }
                },

                Attachments: attachments.ToList()
            );
        });
    }

    public Task<IReadOnlyList<DisputeListItemDto>?> GetOpensAsync(CancellationToken ct)
    {
        return null;
    }

    public Task<IReadOnlyList<DisputeListItemDto>?> GetResolvedAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<DisputeListItemDto>?> GetUnderReviewsAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}