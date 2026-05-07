using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Auctions;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class BidIdConverter : ValueConverter<BidId, Guid>
{
    public BidIdConverter() 
        : base(
            id => id.Value,
            guid =>  BidId.Load(guid)
        ) { }
}
