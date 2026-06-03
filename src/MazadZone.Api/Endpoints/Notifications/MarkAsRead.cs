using Microsoft.AspNetCore.Mvc;
using MediatR;
using MazadZone.Application.Features.Notifications.Commands.MarkAsRead;
using MazadZone.Domain.Notifications;
using MazadZone.Api.Extensions;
using MazadZone.Api.Infrastructure.Binding;

namespace MazadZone.Api.Endpoints.Notifications;

public static class MarkAsRead
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/mark-as-read", HandleAsync)
            .WithName("MarkNotificationAsRead")
            .WithOpenApi()
            .WithSummary("Marks Notification as Read")
            .WithDescription("Marks the specified notification as read by updating its IsRead status to true. **Requires authentication (any role).**")
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
            // TODO(security): The MarkAsReadCommandHandler MUST verify that
            // boundUserId.Value matches the notification's UserId before marking it read.
            var command = new MarkAsReadCommand(notificationId);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => Results.NoContent(),
                errors => errors.ToProblem()
            );
        }
        catch (Exception)
        {
            return Results.Problem("An unexpected error occurred marking the notification as read.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
