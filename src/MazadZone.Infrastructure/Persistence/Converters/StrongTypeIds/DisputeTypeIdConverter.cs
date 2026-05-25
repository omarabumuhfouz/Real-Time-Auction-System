using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MazadZone.Infrastructure.Persistence.Converters;

class DisputeTypeIdConverter : ValueConverter<DisputeTypeId, Guid>
{
    public DisputeTypeIdConverter() 
        : base(id => id.Value, guid =>  DisputeTypeId.From(guid)) { }
}