using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Orders;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class DisputeIdConverter : ValueConverter<DisputeId, Guid>
{
    public DisputeIdConverter() 
        : base(id => id.Value, guid =>  DisputeId.From(guid)) { }
}
