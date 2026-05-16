
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Common.Messaging;
using MazadZone.Domain.Auctions;
namespace MazadZone.Application.Features.Auctions.Queries.GetAuctionById;

public record GetAuctionByIdQuery(AuctionId AuctionId) : ICachedQuery<AuctionDto>
{
    public string CacheKey => $"Auction:{AuctionId}";

    public string[] Tags =>  ["Auction"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}