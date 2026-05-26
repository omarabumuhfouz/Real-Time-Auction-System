using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MazadZone.Domain.Shared.ValueObjects;

namespace MazadZone.Infrastructure.Persistence.Converters;

public class TitleConverter : ValueConverter<Title, string>
{
    public TitleConverter() 
        : base(
            title => title.Value,                         // How to save it to the DB
            dbValue => Title.FromDatabase(dbValue))       // How to read it from the DB
    {
    }
}