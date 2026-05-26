using MazadZone.Domain.Orders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class CommentConverter : ValueConverter<Comment, string>
{
    public CommentConverter() 
        : base(
            comment => comment.Value,
            dbValue => Comment.FromDatabase(dbValue))
    {
    }
}