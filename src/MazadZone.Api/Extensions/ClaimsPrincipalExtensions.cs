using System.Security.Claims;

namespace MazadZone.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        if (principal == null) return Guid.Empty;

        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier) 
                    ?? principal.FindFirstValue("sub");

        return Guid.TryParse(value, out var userId) ? userId : Guid.Empty;
    }
}