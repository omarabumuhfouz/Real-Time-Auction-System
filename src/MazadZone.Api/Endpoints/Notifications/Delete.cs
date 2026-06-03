using Microsoft.AspNetCore.Mvc;
using MediatR;
using MazadZone.Application.Features.Notifications.Commands.DeleteNotification;
using MazadZone.Domain.Notifications;
using MazadZone.Api.Extensions;
using MazadZone.Api.Infrastructure.Binding;

namespace MazadZone.Api.Endpoints.Notifications;

public static class Delete
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("DeleteNotification")
            .WithOpenApi()
            .WithSummary("Deletes a Notification")
            .WithDescription("Soft-deletes a notification by marking it as deleted. The notification will no longer appear in the user's feed. **Requires authentication (any role).**")
            .RequireAuthorization(Policies.Shared)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
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
            // TODO(security): The DeleteNotificationCommandHandler MUST verify that
            // boundUserId.Value matches the notification's UserId before deleting it.
            var command = new DeleteNotificationCommand(notificationId);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => Results.NoContent(),
                errors => errors.ToProblem()
            );
        }
        catch (Exception)
        {
            return Results.Problem("An unexpected error occurred deleting the notification.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
