using MazadZone.Domain.Orders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class RatingConverter : ValueConverter<Rating, int>
{
    public RatingConverter() 
        : base(
            rating => rating.Value,
            dbValue => Rating.FromDatabase(dbValue))
    {
    }
}