/*using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Auctions.Mappers;
using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.Queries;

public static class AuctionMapper
{
    public static AuctionDto ToDto(this Auction entity , SellerDto? sellerDto = null)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AuctionDto(
            Id: entity.Id.Value,
            ItemTitle: entity.Item.Title,
            ItemDescription: entity.Item.Description,
            ImageUrls: entity.Item.Images.Select(i => i.Path).ToList(),
            Seller: entity.Seller.name,
            StartBid: entity.StartBidAmount.Amount,
            MinBidAmount: entity.MinBidAmount.Amount,
            Currency: entity.StartBidAmount.Currency.ToString(),
            StartTime: entity.StartTime,
            EndTime: entity.EndTime,
            RemainderTime: entity.RemainderTime,
            IsActive: entity.IsActive,
            Bids: entity.Bids.ToDtos()
        );
    }

    public static AuctionsListDto ToListDto(this Auction entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AuctionsListDto(
            Id: entity.Id.Value,
            imageUrl: entity.Item.Images.FirstOrDefault()?.Path ?? string.Empty,
            ItemTitle: entity.Item.Title,
            StartingPrice: entity.StartBidAmount.Amount,
            StartTime: entity.StartTime,
            EndTime: entity.EndTime,
            RemainderTime: entity.RemainderTime,
            IsActive: entity.IsActive,
            BidsCount: entity.Bids.Count
        );
    
    }

}*/