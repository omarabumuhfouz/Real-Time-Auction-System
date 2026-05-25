using MazadZone.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class NameConverter : ValueConverter<Name, string>
{
    public NameConverter() 
        : base(
            name => name.Value,
            dbValue => Name.FromDatabase(dbValue))
    {
    }
}