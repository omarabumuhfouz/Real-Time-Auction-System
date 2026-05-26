using System.Security.Claims;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static UserId? GetUserId(this ClaimsPrincipal? principal)
    {
        if (principal == null) return null;

        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier) 
                    ?? principal.FindFirstValue("sub");

        return Guid.TryParse(value, out var userId) ? UserId.Load(userId) : null;
    }
}