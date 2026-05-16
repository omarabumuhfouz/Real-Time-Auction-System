/*using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Auctions.Mappers;

public static class BidMapper
{
    public static BidDto ToDto(this Bid entity)
    {
        //select(a => a.toDto)
        //
        //
        ArgumentNullException.ThrowIfNull(entity);

        return new BidDto(
            Id: entity.Id.Value,
            AuctionId: entity.AuctionId.Value,
            BidderId: entity.Bidder.Name,
            Amount: entity.Amount.Amount,
            Timestamp: entity.PlacedAtUtc
        );
    }

    public static List<BidDto> ToDtos(this IEnumerable<Bid> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return entities.Select(e => e.ToDto()).ToList();
    }
    
}*/