using MazadZone.Domain.Orders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class ResolutionConverter : ValueConverter<Resolution, string>
{
    public ResolutionConverter() 
        : base(
            resolution => resolution.Value,                   // Convert Model -> Provider (Write to DB)
            dbValue => Resolution.FromDatabase(dbValue))      // Convert Provider -> Model (Read from DB)
    {
    }
}