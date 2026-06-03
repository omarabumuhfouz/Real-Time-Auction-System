using Microsoft.AspNetCore.Mvc;
using MediatR;
using MazadZone.Application.Features.Notifications.Commands.MarkAsRead;
using MazadZone.Domain.Notifications;
using MazadZone.Api.Extensions;

namespace MazadZone.Api.Endpoints.Notifications;

public static class MarkAsRead
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/mark-as-read", HandleAsync)
            .WithSummary("Marks Notification as Read")
            .WithDescription("Marks a notification as read by updating its IsRead status.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
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
