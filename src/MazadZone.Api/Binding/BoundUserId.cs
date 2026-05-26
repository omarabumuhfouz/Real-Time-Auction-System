using System.Reflection;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Api.Infrastructure.Binding;

public record struct BoundUserId(UserId Value)
{
    public static ValueTask<BoundUserId?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        // Reuse your existing extension method here
        var userId = context.User.GetUserId(); 
        
        if (userId is null)
        {
            // Returning null tells the Minimal API pipeline that binding failed.
            return ValueTask.FromResult<BoundUserId?>(null);
        }

        return ValueTask.FromResult<BoundUserId?>(new BoundUserId(userId.Value));
    }
}