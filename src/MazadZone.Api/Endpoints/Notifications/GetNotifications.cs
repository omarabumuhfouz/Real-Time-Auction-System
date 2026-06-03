using Microsoft.AspNetCore.Mvc;
using MediatR;
using MazadZone.Application.Features.Notifications.Queries.GetNotifications;
using MazadZone.Application.Features.Notifications.Queries.DTOs;
using MazadZone.Api.Contracts.Notifications;
using MazadZone.Api.Infrastructure.Binding;

namespace MazadZone.Api.Endpoints.Notifications;

public static class GetNotifications
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .WithName("GetNotifications")
            .WithOpenApi()
            .WithSummary("Gets the authenticated user's notifications")
            .WithDescription("Retrieves a paginated list of notifications belonging to the authenticated user, ordered by creation date (newest first). The user identity is taken from the JWT — no UserId parameter is accepted. **Requires authentication (any role).**")
            .RequireAuthorization(Policies.Shared)
            .Produces<NotificationsListDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetNotificationsRequest request,
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        if (request is null || request.PageNumber < 1 || request.PageSize < 1)
        {
            return Results.BadRequest("Invalid pagination parameters.");
        }

        try
        {
            var query = new GetNotificationsQuery(boundUserId.Value, request.PageNumber, request.PageSize);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                success => Results.Ok(success),
                errors => errors.ToProblem()
            );
        }
        catch (Exception)
        {
            return Results.Problem("An unexpected error occurred retrieving notifications.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
