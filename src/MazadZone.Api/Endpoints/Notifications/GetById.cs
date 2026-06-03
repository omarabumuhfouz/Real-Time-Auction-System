using Microsoft.AspNetCore.Mvc;
using MediatR;
using MazadZone.Application.Features.Notifications.Queries.GetNotificationById;
using MazadZone.Application.Features.Notifications.Queries.DTOs;
using MazadZone.Domain.Notifications;
using MazadZone.Api.Extensions;
using MazadZone.Api.Infrastructure.Binding;

namespace MazadZone.Api.Endpoints.Notifications;

public static class GetById
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
            .WithName("GetNotificationById")
            .WithOpenApi()
            .WithSummary("Gets Notification by ID")
            .WithDescription("Retrieves the full details of a specific notification by its unique identifier. **Requires authentication (any role).**")
            .RequireAuthorization(Policies.Shared)
            .Produces<NotificationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return Results.BadRequest("Invalid notification ID.");
        }

        try
        {
            var notificationId = NotificationId.From(id);

            // TODO(security): The GetNotificationByIdQueryHandler MUST verify that
            // boundUserId.Value matches the notification's UserId before returning data.
            var query = new GetNotificationByIdQuery(notificationId);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                success => Results.Ok(success),
                errors => errors.ToProblem()
            );
        }
        catch (Exception)
        {
            return Results.Problem("An unexpected error occurred retrieving the notification.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
