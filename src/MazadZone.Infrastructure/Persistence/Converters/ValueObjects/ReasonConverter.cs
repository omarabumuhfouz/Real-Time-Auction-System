using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Shared.ValueObjects;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class ReasonConverter : ValueConverter<Reason, string>
{
    public ReasonConverter()
        : base(
            reason => reason.Text,
            value => Reason.FromDatabase(value))
    {
    }
}