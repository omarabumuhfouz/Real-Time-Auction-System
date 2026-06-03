using Microsoft.AspNetCore.Mvc;
using MediatR;
using MazadZone.Application.Features.Notifications.Commands.DeleteNotification;
using MazadZone.Domain.Notifications;
using MazadZone.Api.Extensions;

namespace MazadZone.Api.Endpoints.Notifications;

public static class Delete
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", HandleAsync)
            .WithSummary("Deletes a Notification")
            .WithDescription("Soft deletes a notification by marking it as deleted.")
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
