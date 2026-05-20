using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class BidderIdConverter : ValueConverter<BidderId, Guid>
{
    public BidderIdConverter() 
        : base(
            id => id.Value,           // Convert to primitive for SQL Server
            guid => BidderId.Load(guid) // Rehydrate from SQL Server primitive
        ) { }
}
