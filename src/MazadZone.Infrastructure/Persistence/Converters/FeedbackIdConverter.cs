using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Orders;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class FeedbackIdConverter : ValueConverter<FeedbackId, Guid>
{
    public FeedbackIdConverter() 
        : base(id => id.Value, guid =>  FeedbackId.From(guid)) { }
}