using MazadZone.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class DescriptionConverter : ValueConverter<Description, string>
{
    public DescriptionConverter() 
        : base(
            description => description.Value,
            dbValue => Description.FromDatabase(dbValue))
    {
    }
}