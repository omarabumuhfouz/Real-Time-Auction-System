using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Features.Users.Queries.GetProfileSettings;

namespace MazadZone.Api.Endpoints.Users;

public static class GetProfileSettings
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/me/profile-settings", HandleAsync)
           .RequireAuthorization(Policies.Shared)
           .WithOpenApi()
           .WithSummary("Get user profile settings")
           .WithDescription("Retrieves the profile configuration and settings for the authenticated user. The user identity is taken from the JWT. **Requires authentication (any role).**")
           .Produces<ProfileSettingsResponse>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetProfileSettingsQuery(boundUserId.Value), ct);

        return result.Match(
            response => Results.Ok(response),
            error => error.ToProblem());
    }
}