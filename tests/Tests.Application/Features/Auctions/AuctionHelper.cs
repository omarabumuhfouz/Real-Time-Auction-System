using MazadZone.Application.Features.Users.Commands.Ban.Models;
using MazadZone.Application.Features.Users.DTOs;

namespace Tests.Application.Features.Auctions;

public static class AuctionHelper
{
    /// <summary>
    /// Generates a mock list of active auctions with assigned bidders for testing or simulation.
    /// </summary>
    public static List<AuctionBiddersDto> CreateMockActiveAuctions(dynamic bidderOne, dynamic bidderTwo)
    {
        return new List<AuctionBiddersDto>
        {
            new AuctionBiddersDto(
                AuctionId: Guid.NewGuid(),
                Title: "Vintage Watch",
                Bidders: new List<Guid> { bidderOne.Value, bidderTwo.Value }
            ),
            new AuctionBiddersDto(
                AuctionId: Guid.NewGuid(),
                Title: "Gaming Laptop",
                Bidders: new List<Guid> { bidderOne.Value }
            )
        };
    }

    /// <summary>
    /// Generates a mock list of affected auctions when a user is banned or suspended.
    /// </summary>
    public static List<AffectedAuctionDto> CreateMockAffectedAuctions(
        dynamic sellerOneId, 
        dynamic sellerTwoId, 
        dynamic innocentBidderId)
    {
        return new List<AffectedAuctionDto>
        {
            // Auction 1: Has one other innocent bidder competing
            new AffectedAuctionDto(
                Id: Guid.NewGuid(),
                Title: "PlayStation 5",
                SellerId: sellerOneId.Value,
                OtherBidderIds: new HashSet<Guid> { innocentBidderId.Value }
            ),
            
            // Auction 2: Banned user was the ONLY bidder, so no 'OtherBidderIds' to notify
            new AffectedAuctionDto(
                Id: Guid.NewGuid(),
                Title: "Used Car",
                SellerId: sellerTwoId.Value,
                OtherBidderIds: new HashSet<Guid>()
            )
        };
    }
}